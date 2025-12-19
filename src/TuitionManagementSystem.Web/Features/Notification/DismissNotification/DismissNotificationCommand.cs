namespace TuitionManagementSystem.Web.Features.Notification.DismissNotification;

using Ardalis.Result;
using MediatR;

public record DismissNotificationCommand(int UserId, int NotificationId) : IRequest<Result>;
