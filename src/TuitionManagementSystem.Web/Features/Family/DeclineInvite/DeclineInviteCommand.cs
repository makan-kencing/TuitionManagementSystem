namespace TuitionManagementSystem.Web.Features.Family.DeclineInvite;

using Ardalis.Result;
using MediatR;

public record DeclineInviteCommand(int UserId) : IRequest<Result>;
