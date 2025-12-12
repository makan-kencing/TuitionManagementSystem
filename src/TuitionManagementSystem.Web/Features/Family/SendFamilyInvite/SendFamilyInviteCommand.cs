namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using MediatR;

public record SendFamilyInviteCommand(SendFamilyInviteViewModel Invite) : IRequest<Result<SendFamilyInviteResponse>>;
