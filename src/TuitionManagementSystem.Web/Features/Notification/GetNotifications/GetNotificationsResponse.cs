namespace TuitionManagementSystem.Web.Features.Notification.GetNotifications;

using System.Collections.ObjectModel;

public class GetNotificationsResponse(List<GetNotificationResponse> l)
    : ReadOnlyCollection<GetNotificationResponse>(l);

public class GetNotificationResponse
{
    public required int Id { get; set; }

    public required string Message { get; set; }

    public string? ActionUrl { get; set; }

    public required DateTime NotifiedAt { get; set; }
}
