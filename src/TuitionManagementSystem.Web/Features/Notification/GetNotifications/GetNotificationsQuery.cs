namespace TuitionManagementSystem.Web.Features.Notification.GetNotifications;

using Ardalis.Result;
using MediatR;

public record GetNotificationsQuery(int UserId) : IRequest<Result<GetNotificationsResponse>>;
