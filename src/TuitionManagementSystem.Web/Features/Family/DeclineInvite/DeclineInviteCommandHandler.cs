namespace TuitionManagementSystem.Web.Features.Family.DeclineInvite;

using Ardalis.Result;
using MediatR;

public class DeclineInviteCommandHandler : IRequestHandler<DeclineInviteCommand, Result>
{
    public Task<Result> Handle(DeclineInviteCommand command, CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}
