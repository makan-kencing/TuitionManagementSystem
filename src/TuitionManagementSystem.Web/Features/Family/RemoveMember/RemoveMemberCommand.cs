namespace TuitionManagementSystem.Web.Features.Family.RemoveMember;

using Ardalis.Result;
using MediatR;

public record RemoveMemberCommand(int UserId, int RemoveUserId) : IRequest<Result>;
