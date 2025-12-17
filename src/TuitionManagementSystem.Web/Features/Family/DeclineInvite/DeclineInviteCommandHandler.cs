namespace TuitionManagementSystem.Web.Features.Family.DeclineInvite;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Notification;

public class DeclineInviteCommandHandler(ApplicationDbContext db) : IRequestHandler<DeclineInviteCommand, Result>
{
    public async Task<Result> Handle(DeclineInviteCommand command, CancellationToken cancellationToken)
    {
        var invite = await db.FamilyInvites
            .Where(i => i.User.Account.Id == command.UserId)
            .Where(i => i.Status == InviteStatus.Pending)
            .FirstOrDefaultAsync(cancellationToken);

        if (invite is null)
        {
            return Result.NotFound();
        }

        invite.Status = InviteStatus.Declined;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
