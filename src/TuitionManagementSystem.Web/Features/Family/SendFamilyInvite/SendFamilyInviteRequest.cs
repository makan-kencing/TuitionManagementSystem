namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using MediatR;

public record SendFamilyInviteRequest(SendFamilyInviteViewModel Invite) : IRequest<Result<SendFamilyInviteResponse>>;
