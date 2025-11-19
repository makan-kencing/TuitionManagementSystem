namespace TuitionManagementSystem.Web.Features.Accounts.CheckEmail;

using Ardalis.Result;
using MediatR;

public record CheckEmailRequest(string Email) : IRequest<Result<CheckEmailResponse>>;
