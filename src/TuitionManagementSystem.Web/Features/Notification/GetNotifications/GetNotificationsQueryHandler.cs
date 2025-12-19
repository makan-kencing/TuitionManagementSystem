namespace TuitionManagementSystem.Web.Features.Notification.GetNotifications;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetNotificationsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetNotificationsQuery, Result<GetNotificationsResponse>>
{
    public async Task<Result<GetNotificationsResponse>> Handle(GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await db.Notifications
            .Where(n => n.User.Id == request.UserId)
            .Where(n => n.ReadAt == null)
            .Select(n => new GetNotificationResponse
            {
                Id = n.Id,
                Message = n.Message,
                ActionUrl = n.ActionUrl,
                NotifiedAt = n.NotifiedAt
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new GetNotificationsResponse(notifications));
    }
}
