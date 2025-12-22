namespace TuitionManagementSystem.Web.Features.Checkout.OnCheckoutSuccess;

using Microsoft.AspNetCore.Mvc;
using Payment.GetPaymentDetails;

public class PaymentSuccessViewModel
{
    public required string Email { get; init; }

    public required string? Name { get; init; }

    public required ControllerContext ControllerContext { get; init; }

    public required ICollection<PaidInvoice> Invoices { get; init; }
}
