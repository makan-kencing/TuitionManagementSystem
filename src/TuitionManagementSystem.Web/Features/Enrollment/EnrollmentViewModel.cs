namespace TuitionManagementSystem.Web.Models.ViewModels;

using System.ComponentModel.DataAnnotations;
using Class;

public class EnrollmentViewModel
{
    public int? EnrollmentId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }

    public DateTime? EnrolledAt { get; set; }

    public Enrollment.EnrollmentStatus? Status { get; set; }

    public string? CurrentUserType { get; set; }
}

