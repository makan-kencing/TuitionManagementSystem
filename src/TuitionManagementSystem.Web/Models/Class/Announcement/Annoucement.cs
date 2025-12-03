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

    public required int? CourseId { get; set; }
    [ForeignKey(nameof(CourseId))]
    public required Course Course { get; set; }

    public Session? RelatedSession { get; set; }

    public required Teacher CreatedBy { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime CreatedAt { get; set; } = DateTime.Now;

    public required DateTime UpdatedAt { get; set; }

    public required DateTime PublishedAt { get; set; }  // for drafting purposes

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<AnnouncementFile> Attachments { get; set; } = [];
}

public class AnnouncementFile
{
    [Key]
    public int Id { get; set; }

    public required int AnnouncementId { get; set; }
    public required Announcement Announcement { get; set; }

    public required int FileId { get; set; }
    public required File File { get; set; }
}
