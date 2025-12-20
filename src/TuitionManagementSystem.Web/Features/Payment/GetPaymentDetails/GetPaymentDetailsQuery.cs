namespace TuitionManagementSystem.Web.Features.Payment.GetPaymentDetails;

using Ardalis.Result;
using MediatR;

public record GetPaymentDetailsQuery(int UserId, int PaymentId) : IRequest<Result<GetPaymentDetailsResponse>>;
