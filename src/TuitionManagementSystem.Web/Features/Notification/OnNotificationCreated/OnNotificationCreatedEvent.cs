namespace TuitionManagementSystem.Web.Features.Notification.OnNotificationCreated;

using MediatR;
using Models.Notification;

public record OnNotificationCreatedEvent(Notification Notification) : INotification;
