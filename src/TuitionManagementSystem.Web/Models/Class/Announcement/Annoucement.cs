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

    public ICollection<File> Attachments { get; set; } = new List<File>();

    public required Teacher CreatedBy { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }

    public required DateTime PublishedAt { get; set; }  // for drafting purposes

    public required Course Course { get; set; }

    public Session? RelatedSession { get; set; }

    public DateTime? DeletedAt { get; set; }
}
