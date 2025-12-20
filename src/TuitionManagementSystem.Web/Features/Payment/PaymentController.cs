namespace TuitionManagementSystem.Web.Features.Payment;

using Abstractions;
using Ardalis.Result;
using GetPaymentDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;

public class PaymentController(IMediator mediator) : WebController
{
    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var payment = await mediator.Send(new GetPaymentDetailsQuery(
            this.User.GetUserId(), id));

        if (payment.IsNotFound())
        {
            return this.NotFound();
        }

        if (payment.IsUnauthorized())
        {
            return this.Challenge();
        }

        if (payment.IsForbidden())
        {
            return this.Forbid();
        }

        return this.View(new PaymentDetailsViewModel
        {
            Details = payment.Value
        });
    }

    public class PaymentDetailsViewModel
    {
        public required GetPaymentDetailsResponse Details { get; init; }
    }
}
