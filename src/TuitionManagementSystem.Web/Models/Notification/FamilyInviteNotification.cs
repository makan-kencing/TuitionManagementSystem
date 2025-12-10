namespace TuitionManagementSystem.Web.Models.Notification;

using System.ComponentModel.DataAnnotations.Schema;

public class FamilyInviteNotification : Notification
{
    [ForeignKey(nameof(FamilyInvite) + "Id")]
    public required FamilyInvite FamilyInvite { get; set; }
}
