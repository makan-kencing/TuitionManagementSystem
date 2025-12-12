namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;

public class SendFamilyInviteCommandHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SendFamilyInviteCommand, Result<SendFamilyInviteResponse>>
{
    public Task<Result<SendFamilyInviteResponse>> Handle(
        SendFamilyInviteCommand command,
        CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}

