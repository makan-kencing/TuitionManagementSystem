namespace TuitionManagementSystem.Web.Features.Checkout;

using System.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Abstractions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Models.Payment;
using Options;
using Stripe;
using Stripe.Checkout;
using PaymentMethod = Models.Payment.PaymentMethod;

public class CheckoutController : WebController
{
    private readonly StripeOptions options;
    private readonly StripeClient client;
    private readonly ApplicationDbContext db;

    public CheckoutController(IConfiguration configuration, ApplicationDbContext db)
    {
        this.options = configuration.GetSection("Stripe")
                           .Get<StripeOptions>()
                       ?? throw new ConfigurationErrorsException("Stripe configurations not found.");
        this.client = new StripeClient(this.options.SecretKey);
        this.db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Index(ICollection<int> invoiceIds)
    {
        var items = await this.db.Invoices
            .Where(i => invoiceIds.Contains(i.Id))
            .Select(i => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = i.Amount,
                    Currency = "myr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Enrollment fee for {i.Enrollment.Course.Name}",
                        Metadata = { { "id", i.Id.ToString(CultureInfo.InvariantCulture) } }
                    }
                },
                Quantity = 1
            }).ToListAsync();
        items.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = items.Sum(i => i.PriceData.UnitAmountDecimal) * 0.02m,
                Currency = "myr",
                ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "Processing Fee (2%)" }
            }
        });

        var sessionOptions = new SessionCreateOptions
        {
            LineItems = items,
            Mode = "payment",
            SuccessUrl = this.Url.Action("Complete", "Checkout", null, protocol: this.Request.Scheme)
                         + "?session_id={CHECKOUT_SESSION_ID}"
        };

        var session = await this.client.V1.Checkout.Sessions.CreateAsync(sessionOptions);

        return this.SeeOther(session.Url);
    }

    [HttpGet]
    public async Task<IActionResult> Complete([FromQuery] string sessionId)
    {
        var sessionOptions = new SessionGetOptions
        {
            Expand =
            [
                "line_items",
                "payment_intent",
                "payment_intent.payment_method"
            ]
        };

        var session = await this.client.V1.Checkout.Sessions.GetAsync(sessionId, sessionOptions);

        if (session.PaymentStatus != "unpaid")
        {
            PaymentMethod paymentMethod = session.PaymentIntent.PaymentMethod.Type switch
            {
                "card" => session.PaymentIntent.PaymentMethod.Card.ToMethod(),
                "fpx" => session.PaymentIntent.PaymentMethod.Fpx.ToMethod(),
                var method => new GenericPaymentMethod { Generic = method }
            };

            var invoiceIds = session.LineItems
                .Select(li => li.Product.Metadata.TryGetValue("id", out var value) ? value : null)
                .OfType<string>()
                .Select(s => int.Parse(s, CultureInfo.InvariantCulture))
                .ToList();

            var payment = new Payment
            {
                Amount = session.PaymentIntent.Amount,
                Method = paymentMethod,
                Invoices = await this.db.Invoices
                    .Where(i => invoiceIds.Contains(i.Id))
                    .ToListAsync()
            };
            this.db.Payments.Add(payment);
            await this.db.SaveChangesAsync();
        }

        return this.RedirectToAction("InvoiceHistory", "Invoice");
    }
}

public static class StripePaymentMethodExtensions
{
    public static CardPaymentMethod ToMethod(this PaymentMethodCard card) =>
        new() { Brand = card.Brand, Last4 = card.Last4 };

    public static BankPaymentMethod ToMethod(this PaymentMethodFpx fpx) =>
        new() { Bank = fpx.Bank };
}
