namespace TuitionManagementSystem.Web.Features.Checkout;

using System.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Abstractions;
using Infrastructure.Persistence;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
using Models.Payment;
using Options;
using Services;
using Stripe;
using Stripe.Checkout;
using PaymentMethod = Models.Payment.PaymentMethod;

public class CheckoutController : WebController
{
    private readonly StripeOptions options;
    private readonly StripeClient client;
    private readonly ApplicationDbContext db;
    private readonly IInvoiceService invoiceService;

    public CheckoutController(IConfiguration configuration, ApplicationDbContext db, IInvoiceService invoiceService)
    {
        this.options = configuration.GetSection("Stripe")
                           .Get<StripeOptions>()
                       ?? throw new ConfigurationErrorsException("Stripe configurations not found.");
        this.client = new StripeClient(this.options.SecretKey);
        this.db = db;
        this.invoiceService = invoiceService;
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromForm] List<int> invoiceIds)
    {
        if (invoiceIds.Count == 0)
        {
            return this.BadRequest("No invoices selected.");
        }

        var invoices = await this.db.Invoices
            .Where(i => invoiceIds.Contains(i.Id))
            .Select(i => new { i.Id, i.Amount, CourseName = i.Enrollment.Course.Name })
            .ToListAsync();

        var items = invoices.Select(i => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = (long)Math.Round(i.Amount * 100),
                Currency = "myr",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = $"Enrollment fee for {i.CourseName}",
                    Metadata = new Dictionary<string, string>
                    {
                        ["invoice_id"] = i.Id.ToString(CultureInfo.InvariantCulture)
                    }
                }
            },
            Quantity = 1
        }).ToList();

        var subtotal = items.Sum(i => i.PriceData.UnitAmountDecimal);

        items.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = (long)Math.Round((decimal)(subtotal * 0.02m)!),
                Currency = "myr",
                ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "Processing Fee (2%)" }
            },
            Quantity = 1
        });

        var sessionOptions = new SessionCreateOptions
        {
            LineItems = items,
            Mode = "payment",
            SuccessUrl = this.Url.Action("Complete", "Checkout", null, this.Request.Scheme)
                         + "?session_id={CHECKOUT_SESSION_ID}",
            Metadata = new Dictionary<string, string> { ["invoice_ids"] = string.Join(",", invoiceIds) }
        };

        var session = await this.client.V1.Checkout.Sessions.CreateAsync(sessionOptions);
        return this.SeeOther(session.Url);
    }

    [HttpGet]
    public async Task<IActionResult> Complete([FromQuery(Name = "session_id")] string sessionId)
    {
        var sessionOptions = new SessionGetOptions
        {
            Expand =
            [
                "line_items",
                "line_items.data.price.product",
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

            List<int> invoiceIds;

            if (session.Metadata != null && session.Metadata.ContainsKey("invoice_ids"))
            {
                invoiceIds = session.Metadata["invoice_ids"]
                    .Split(',')
                    .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                    .ToList();
            }
            else
            {
                var invoiceIdStrings = session.LineItems.Data
                    .Where(li => li.Price?.Product != null &&
                                 li.Price.Product.Metadata != null)
                    .SelectMany(li => li.Price.Product.Metadata
                        .Where(kvp => kvp.Key == "invoice_id")
                        .Select(kvp => kvp.Value))
                    .Distinct()
                    .ToList();

                invoiceIds = invoiceIdStrings
                    .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                    .ToList();
            }

            if (invoiceIds.Count == 0)
            {
                return this.BadRequest("No invoice IDs found in payment session");
            }

            var invoices = await this.db.Invoices
                .Where(i => invoiceIds.Contains(i.Id))
                .ToListAsync();

            if (invoices.Count == 0)
            {
                return this.BadRequest($"No invoices found for IDs: {string.Join(", ", invoiceIds)}");
            }

            var payment = new Payment
            {
                Amount = (decimal)session.PaymentIntent.Amount / 100, Method = paymentMethod, Invoices = invoices
            };

            this.db.Payments.Add(payment);
            await this.db.SaveChangesAsync();

            var result = await this.invoiceService.MarkInvoicesAsPaidAsync(
                invoiceIds,
                payment.Id,
                this.HttpContext.RequestAborted);

            if (!result.IsSuccess)
            {
                return this.BadRequest(result.Errors);
            }

            return this.RedirectToAction("Index", "Payment", new { id = payment.Id });
        }

        return this.RedirectToAction("ListInvoices", "Invoice");
    }
}

public static class StripePaymentMethodExtensions
{
    public static CardPaymentMethod ToMethod(this PaymentMethodCard card) =>
        new() { Brand = card.Brand, Last4 = card.Last4 };

    public static BankPaymentMethod ToMethod(this PaymentMethodFpx fpx) =>
        new() { Bank = fpx.Bank };
}
