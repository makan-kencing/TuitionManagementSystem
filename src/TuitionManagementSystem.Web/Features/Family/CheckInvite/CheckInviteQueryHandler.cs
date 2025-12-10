namespace TuitionManagementSystem.Web.Features.Family.CheckInvite;

using Ardalis.Result;
using AutoMapper;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Notification;
using Services.Auth.Extensions;

public class CheckInviteQueryHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper) : IRequestHandler<CheckInviteQuery, Result<CheckInviteResponse>>
{
    public async Task<Result<CheckInviteResponse>> Handle(
        CheckInviteQuery query,
        CancellationToken cancellationToken)
    {
        var invite = await db.FamilyInvites
            .Include(i => i.Family)
            .Include(i => i.Requester)
            .Where(i => i.Status == InviteStatus.Pending)
            .Where(i => i.User.Account.Id == httpContextAccessor.HttpContext!.User.GetUserId())
            .FirstOrDefaultAsync(cancellationToken);

        if (invite == null)
        {
            return Result.NotFound();
        }

        return Result.Success(mapper.Map<CheckInviteResponse>(invite));
    }
}
