namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using User;

public class Submission
{
    [Key]
    public int Id { get; set; }

    public required Student Student { get; set; }

    public required Assignment Assignment { get; set; }

    [StringLength(500)]
    public string? Content { get; set; }

    public ICollection<File> Attachments { get; set; }= new List<File>();

    public int? Grade { get; set; }

    public DateTime SubmittedAt { get; set; }= DateTime.Now;
}
