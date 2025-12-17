namespace TuitionManagementSystem.Web.Features.Family.OnFamilyInviteCreated;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Notification;
using Notification.OnNotificationCreated;

public class CreateFamilyInviteNotificationHandler(
    IMediator mediator,
    ApplicationDbContext db) : INotificationHandler<OnFamilyInviteCreatedEvent>
{
    public async Task Handle(
        OnFamilyInviteCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        var notification = await db.FamilyInvites
            .Where(fi => fi.Id == @event.FamilyInviteId)
            .Select(fi => new Notification
            {
                Message = $"You're invited to join {fi.Family.Name}",
                ActionUrl = new Uri("/family"),
                UserId = fi.UserId
            })
            .FirstAsync(cancellationToken);

        await db.Notifications.AddAsync(notification, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new OnNotificationCreatedEvent(notification), cancellationToken);
    }
}
