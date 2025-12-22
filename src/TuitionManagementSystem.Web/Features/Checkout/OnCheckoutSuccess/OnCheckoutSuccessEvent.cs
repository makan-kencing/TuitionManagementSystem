namespace TuitionManagementSystem.Web.Features.Checkout.OnCheckoutSuccess;

using MediatR;
using Microsoft.AspNetCore.Mvc;

public record OnCheckoutSuccessEvent(int PaymentId, string Email, string? Name, ControllerContext ControllerContext) : INotification;
