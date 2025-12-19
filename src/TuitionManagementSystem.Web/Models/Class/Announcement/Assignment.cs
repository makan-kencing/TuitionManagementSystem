namespace TuitionManagementSystem.Web.Models.Class.Announcement;

public class Assignment : Announcement
{
    public DateTime? DueAt { get; set; }

    public ICollection<Submission> Submissions { get; set; } = [];
}
