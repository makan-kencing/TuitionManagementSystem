namespace TuitionManagementSystem.Web.Features.Notification.DismissNotification;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DismissNotificationCommandHandler(ApplicationDbContext db) : IRequestHandler<DismissNotificationCommand, Result>
{
    public async Task<Result> Handle(DismissNotificationCommand request, CancellationToken cancellationToken)
    {
        await db.Notifications
            .Where(n => n.Id == request.NotificationId)
            .Where(n => n.User.Id == request.UserId)
            .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(n => n.ReadAt, DateTime.UtcNow),
                cancellationToken);
        return Result.Success();
    }
}
