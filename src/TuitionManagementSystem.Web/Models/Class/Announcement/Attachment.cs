namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using User;

public class Attachment
{
    [Key]
    public int Id { get; set; }

    public required Uri Uri { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public required User CreatedBy { get; set; }
}
