namespace TuitionManagementSystem.Web.Features.Accounts.RemoveSession;

using Ardalis.Result;
using MediatR;

public record RemoveSessionRequest(Guid SessionId) : IRequest<Result<RemoveSessionResponse>>;
