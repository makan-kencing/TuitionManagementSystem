namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(SubmissionId), nameof(FileId), IsUnique = true)]
public class SubmissionFile
{
    [Key]
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    public int FileId { get; set; }

    public Submission Submission { get; set; } = null!;

    public File File { get; set; } = null!;
}
