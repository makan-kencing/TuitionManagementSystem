namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using MediatR;

public record SendFamilyInviteCommand(string Username) : IRequest<Result<SendFamilyInviteResponse>>;
