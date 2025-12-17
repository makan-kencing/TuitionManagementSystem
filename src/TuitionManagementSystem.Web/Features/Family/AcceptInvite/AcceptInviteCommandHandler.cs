namespace TuitionManagementSystem.Web.Features.Family.AcceptInvite;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Notification;
using Models.User;

public class AcceptInviteCommandHandler(ApplicationDbContext db) : IRequestHandler<AcceptInviteCommand, Result>
{
    public async Task<Result> Handle(AcceptInviteCommand command, CancellationToken cancellationToken)
    {
        var invite = await db.FamilyInvites
            .Where(i => i.User.Id == command.UserId)
            .Where(i => i.Status == InviteStatus.Pending)
            .FirstOrDefaultAsync(cancellationToken);

        if (invite is null)
        {
            return Result.NotFound();
        }

        invite.Status = InviteStatus.Accepted;
        invite.Family.Members.Add(new FamilyMember
        {
            UserId = invite.UserId
        });

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
