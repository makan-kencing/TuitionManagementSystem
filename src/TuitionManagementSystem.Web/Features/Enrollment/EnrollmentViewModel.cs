namespace TuitionManagementSystem.Web.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class EnrollmentViewModel
{
    public int? EnrollmentId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }
}

