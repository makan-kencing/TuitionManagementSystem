using TuitionManagementSystem.Web.Features.Family.OnFamilyInviteCreated;
using TuitionManagementSystem.Web.Services.Auth.Extensions;

namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Notification;
using Models.User;

public class SendFamilyInviteCommandHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor,
    IMediator mediator) : IRequestHandler<SendFamilyInviteCommand, Result<SendFamilyInviteResponse>>
{
    public async Task<Result<SendFamilyInviteResponse>> Handle(
        SendFamilyInviteCommand command,
        CancellationToken cancellationToken)
    {
        var accountId = httpContextAccessor.HttpContext?.User.GetUserId();

        if (accountId is null)
        {
            return Result.Unauthorized();
        }

        var parent = await db.Parents
            .Where(u => u.Account.Id == accountId)
            .FirstOrDefaultAsync(cancellationToken);

        switch (parent)
        {
            case null:
                return Result.Unauthorized();
            case { Family: null }:
                return Result.Invalid();
        }

        var invited = await db.Users
            .Where(u => u.Account.Username == command.Username)
            .FirstOrDefaultAsync(cancellationToken);

        switch (invited)
        {
            case null:
                return Result.NotFound();
            case not IHasFamily or IHasFamily { Family: not null }:
                return Result.Forbidden();
        }

        if (await db.FamilyInvites
                .Where(i => i.User.Id == invited.Id)
                .Where(i => i.Status == InviteStatus.Pending)
                .AnyAsync(cancellationToken))
        {
            return Result.Conflict();
        }

        var invite = new FamilyInvite
        {
            Family = parent.Family,
            User = invited,
            Requester = parent
        };
        await db.FamilyInvites.AddAsync(invite, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new OnFamilyInviteCreatedEvent(invite), cancellationToken);

        return Result.Success(new SendFamilyInviteResponse());
    }
}
