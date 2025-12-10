namespace TuitionManagementSystem.Web.Features.Family.CheckInvite;

using Ardalis.Result;
using MediatR;

public record CheckInviteQuery : IRequest<Result<CheckInviteResponse>>;
