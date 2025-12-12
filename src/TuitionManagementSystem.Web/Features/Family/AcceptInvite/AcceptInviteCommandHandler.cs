namespace TuitionManagementSystem.Web.Features.Family.AcceptInvite;

using Ardalis.Result;
using MediatR;

public class AcceptInviteCommandHandler : IRequestHandler<AcceptInviteCommand, Result>
{
    public async Task<Result> Handle(AcceptInviteCommand command, CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}
