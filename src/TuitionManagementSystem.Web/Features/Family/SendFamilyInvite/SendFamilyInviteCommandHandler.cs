namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Notification;
using OnFamilyInviteCreated;
using Services.Auth.Extensions;

public class SendFamilyInviteCommandHandler(ApplicationDbContext db, IMediator mediator)
    : IRequestHandler<SendFamilyInviteCommand, Result>
{
    public async Task<Result> Handle(
        SendFamilyInviteCommand command,
        CancellationToken cancellationToken)
    {
        var parent = await db.FamilyMembers
            .Where(fm => fm.User.Id == command.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (parent is null)
        {
            return Result.NotFound();
        }

        var invited = await db.Users
            .Where(u => u.Account.Username == command.Username)
            .Include(u => u.Family)
            .FirstOrDefaultAsync(cancellationToken);

        switch (invited)
        {
            case null:
            case { Family: not null }:
                return Result.Forbidden();
        }

        if (await db.FamilyInvites
                .Where(i => i.User.Id == invited.Id)
                .Where(i => i.Status == InviteStatus.Pending)
                .AnyAsync(cancellationToken))
        {
            return Result.Conflict();
        }

        var invite = new FamilyInvite { FamilyId = parent.FamilyId, User = invited, RequesterId = parent.UserId };
        await db.FamilyInvites.AddAsync(invite, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new OnFamilyInviteCreatedEvent(invite.Id), cancellationToken);

        return Result.Success();
    }
}
