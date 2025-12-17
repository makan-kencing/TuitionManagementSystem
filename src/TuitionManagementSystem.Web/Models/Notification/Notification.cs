namespace TuitionManagementSystem.Web.Models.Notification;

using System.ComponentModel.DataAnnotations;
using User;

public class Notification
{
    [Key] public int Id { get; set; }

    [StringLength(500)]
    public required string Message { get; set; }

    [StringLength(500)]
    public string? ActionUrl { get; set; }

    public int UserId { get; set; }

    public DateTime NotifiedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public User User { get; set; } = null!;
}
