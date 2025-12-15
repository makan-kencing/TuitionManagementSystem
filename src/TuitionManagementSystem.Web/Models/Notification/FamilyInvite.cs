namespace TuitionManagementSystem.Web.Models.Notification;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using User;

[Index(nameof(FamilyId), nameof(UserId))]
public class FamilyInvite
{
    [Key]
    public int Id { get; set; }

    public int FamilyId { get; set; }

    public int UserId { get; set; }

    public int RequesterId { get; set; }

    public InviteStatus Status { get; set; } = InviteStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public Family Family { get; set; } = null!;

    public User User { get; set; } = null!;

    public Parent Requester { get; set; } = null!;
}

public enum InviteStatus
{
    Pending,
    Accepted,
    Declined
}
