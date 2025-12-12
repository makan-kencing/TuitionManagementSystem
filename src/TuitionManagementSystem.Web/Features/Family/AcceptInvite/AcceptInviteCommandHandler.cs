using Microsoft.EntityFrameworkCore;
using TuitionManagementSystem.Web.Infrastructure.Persistence;
using TuitionManagementSystem.Web.Models.Notification;
using TuitionManagementSystem.Web.Services.Auth.Extensions;

namespace TuitionManagementSystem.Web.Features.Family.AcceptInvite;

using Ardalis.Result;
using MediatR;

public class AcceptInviteCommandHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<AcceptInviteCommand, Result>
{
    public async Task<Result> Handle(AcceptInviteCommand command, CancellationToken cancellationToken)
    {
        var accountId = httpContextAccessor.HttpContext?.User.GetUserId();
        if (accountId == null)
        {
            return Result.Unauthorized();
        }

        var invite = await db.FamilyInvites
            .Where(i => i.User.Account.Id == accountId)
            .Where(i => i.Status == InviteStatus.Pending)
            .Include(i => i.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (invite is null)
        {
            return Result.NotFound();
        }

        invite.Status = InviteStatus.Accepted;
        invite.User.Family = invite.Family;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
