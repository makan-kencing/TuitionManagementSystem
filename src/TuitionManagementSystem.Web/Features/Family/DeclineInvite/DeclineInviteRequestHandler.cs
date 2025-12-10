namespace TuitionManagementSystem.Web.Features.Family.DeclineInvite;

using Ardalis.Result;
using MediatR;

public class DeclineInviteRequestHandler : IRequestHandler<DeclineInviteRequest, Result>
{
    public Task<Result> Handle(DeclineInviteRequest request, CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}
