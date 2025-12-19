namespace TuitionManagementSystem.Web.Features.Notification.MarkAllNotificationsDelivered;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class MarkAllNotificationsDeliveredCommandHandler(ApplicationDbContext db)
    : IRequestHandler<MarkAllNotificationsDeliveredCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsDeliveredCommand request, CancellationToken cancellationToken)
    {
        await db.Notifications
            .Where(n => n.User.Id == request.UserId)
            .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(n => n.DeliveredAt, DateTime.UtcNow),
                cancellationToken);
        return Result.Success();
    }
}
