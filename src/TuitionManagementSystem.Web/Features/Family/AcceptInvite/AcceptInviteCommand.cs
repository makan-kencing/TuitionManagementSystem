namespace TuitionManagementSystem.Web.Features.Family.AcceptInvite;

using Ardalis.Result;
using MediatR;

public record AcceptInviteCommand(int UserId) : IRequest<Result>;
