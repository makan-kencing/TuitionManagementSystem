namespace TuitionManagementSystem.Web.Features.Homework;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class MakeSubmissionViewModel
{
    public int AssignmentId { get; set; }

    [DisplayName("Content")]
    [StringLength(500)]
    public string? Content { get; set; }

    [DisplayName("Attachments")]
    public ICollection<int> FileIds { get; set; } = [];

}
