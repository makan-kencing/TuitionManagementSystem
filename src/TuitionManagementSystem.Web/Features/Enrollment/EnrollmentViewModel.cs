namespace TuitionManagementSystem.Web.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class EnrollmentViewModel
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }
}
