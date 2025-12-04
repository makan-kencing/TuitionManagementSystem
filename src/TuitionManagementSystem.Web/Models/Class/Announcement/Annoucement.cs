namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Announcement : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Title { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [ForeignKey(nameof(Course) + "Id")]
    public required Course Course { get; set; }

    [ForeignKey(nameof(RelatedSession) + "Id")]
    public Session? RelatedSession { get; set; }

    [ForeignKey(nameof(CreatedBy) + "Id")]
    public required Teacher CreatedBy { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; }

    public required DateTime PublishedAt { get; set; }  // for drafting purposes

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<AnnouncementFile> Attachments { get; set; } = [];
}
