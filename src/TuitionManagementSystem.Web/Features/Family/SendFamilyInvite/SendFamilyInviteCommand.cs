namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using MediatR;

public record SendFamilyInviteCommand(int UserId, string Username) : IRequest<Result>;
