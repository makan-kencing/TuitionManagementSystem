namespace TuitionManagementSystem.Web.Features.Notification.MarkAllNotificationsDelivered;

using Ardalis.Result;
using MediatR;

public record MarkAllNotificationsDeliveredCommand(int UserId) : IRequest<Result>;
