namespace TuitionManagementSystem.Web.Features.Family.OnFamilyInviteCreated;

using MediatR;
using Models.Notification;
using Notification.OnNotificationCreated;
using Infrastructure.Persistence;

public class CreateFamilyInviteNotificationHandler(
    IMediator mediator,
    ApplicationDbContext db) : INotificationHandler<OnFamilyInviteCreatedEvent>
{
    public async Task Handle(
        OnFamilyInviteCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Message = $"You're invited to join {@event.Invite.Family.Name}",
            ActionUrl = new Uri("/family"),
            User = @event.Invite.User
        };

        await db.Notifications.AddAsync(notification, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new OnNotificationCreatedEvent(notification), cancellationToken);
    }
}
