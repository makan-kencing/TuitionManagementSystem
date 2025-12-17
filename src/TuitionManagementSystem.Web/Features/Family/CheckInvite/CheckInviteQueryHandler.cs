namespace TuitionManagementSystem.Web.Features.Family.CheckInvite;

using Ardalis.Result;
using AutoMapper;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Notification;

public class CheckInviteQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<CheckInviteQuery, Result<CheckInviteResponse>>
{
    public async Task<Result<CheckInviteResponse>> Handle(
        CheckInviteQuery query,
        CancellationToken cancellationToken)
    {
        var invite = await db.FamilyInvites
            .Where(i => i.Status == InviteStatus.Pending)
            .Where(i => i.User.Account.Id == query.UserId)
            .Select(fi => new CheckInviteResponse
            {
                Family = new Family { Name = fi.Family.Name },
                Requester = new InviteParent
                {
                    AccountUsername = fi.Requester.Account.Username,
                    AccountDisplayName = fi.Requester.Account.DisplayName
                },
                RequestedAt = fi.RequestedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (invite == null)
        {
            return Result.NotFound();
        }

        return Result.Success(invite);
    }
}
