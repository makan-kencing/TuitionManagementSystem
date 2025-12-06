namespace TuitionManagementSystem.Web.Models.Notification;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public required string Message { get; set; }

    public Uri? ActionUrl { get; set; }

    [ForeignKey(nameof(User) + "Id")]
    public required User User { get; set; }

    public required DateTime NotifiedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }
}
