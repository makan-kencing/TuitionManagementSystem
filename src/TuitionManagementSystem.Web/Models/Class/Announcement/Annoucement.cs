namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using User;

public class Announcement : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Title { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    public int CourseId { get; set; }

    public int? RelatedSessionId { get; set; }

    public int CreatedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; } // for drafting purposes

    public Course Course { get; set; } = null!;

    public Session? RelatedSession { get; set; }

    public Teacher CreatedBy { get; set; } = null!;

    public ICollection<AnnouncementFile> Attachments { get; set; } = [];

    public DateTime? DeletedAt { get; set; }
}
