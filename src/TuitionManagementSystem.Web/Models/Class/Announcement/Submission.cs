namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using User;

[Index(nameof(StudentId), nameof(AssignmentId), IsUnique = true)]
public class Submission
{
    [Key]
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int AssignmentId { get; set; }

    [StringLength(500)]
    public string? Content { get; set; }

    public int? Grade { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Student Student { get; set; } = null!;

    public Assignment Assignment { get; set; } = null!;

    public ICollection<SubmissionFile> Attachments { get; set; } = [];
}
