namespace TuitionManagementSystem.Web.Features.Homework;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class MakeAnnouncementViewModel
{
    [DisplayName("CourseId")]
    public int CourseId { get; set; }

    [DisplayName("Title")]
    [StringLength(50)]
    public string Title { get; set; }

    [DisplayName("Description")]
    [StringLength(2000)]
    public string? Description { get; set; }

    [DisplayName("Attachments")]
    public ICollection<int> FileIds { get; set; } = [];

    [DisplayName("DueAt")]
    public DateTime DueAt { get; set; }
}
