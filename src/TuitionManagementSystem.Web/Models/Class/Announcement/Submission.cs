namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Submission
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Student) + "Id")]
    public required Student Student { get; set; }

    [ForeignKey(nameof(Assignment) + "Id")]
    public required Assignment Assignment { get; set; }

    [StringLength(500)]
    public string? Content { get; set; }

    public int? Grade { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime SubmittedAt { get; set; } = DateTime.Now;

    public virtual ICollection<SubmissionFile> Attachments { get; set; } = [];
}
