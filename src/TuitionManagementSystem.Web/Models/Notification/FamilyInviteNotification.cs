namespace TuitionManagementSystem.Web.Models.Notification;

public class FamilyInviteNotification : Notification
{
    public int FamilyInviteId { get; set; }

    public required FamilyInvite FamilyInvite { get; set; }
}
