namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;

public class SubmissionFile
{
    [Key]
    public int Id { get; set; }

    public required int SubmissionId { get; set; }
    public required Submission Submission { get; set; }

    public required int FileId { get; set; }
    public required File File { get; set; }
}
