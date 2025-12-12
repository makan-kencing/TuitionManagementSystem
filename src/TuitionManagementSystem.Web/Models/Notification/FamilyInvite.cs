namespace TuitionManagementSystem.Web.Models.Notification;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class FamilyInvite
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Family) + "Id")]
    public required Family Family { get; set; }

    [ForeignKey(nameof(User) + "Id")]
    public required User User { get; set; }

    public required Parent Requester { get; set; }

    public InviteStatus Status { get; set; } = InviteStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}

public enum InviteStatus
{
    Pending,
    Accepted,
    Declined
}
