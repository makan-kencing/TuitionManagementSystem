namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;

public class SendFamilyInviteRequestHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SendFamilyInviteRequest, Result<SendFamilyInviteResponse>>
{
    public Task<Result<SendFamilyInviteResponse>> Handle(
        SendFamilyInviteRequest request,
        CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}

