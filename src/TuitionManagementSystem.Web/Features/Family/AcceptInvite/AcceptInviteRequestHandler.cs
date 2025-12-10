namespace TuitionManagementSystem.Web.Features.Family.AcceptInvite;

using Ardalis.Result;
using MediatR;

public class AcceptInviteRequestHandler : IRequestHandler<AcceptInviteRequest, Result>
{
    public async Task<Result> Handle(AcceptInviteRequest request, CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}
