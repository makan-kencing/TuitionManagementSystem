namespace TuitionManagementSystem.Web.Features.Enrollment.ViewEnrollment;

using Models.Class;

public class ViewEnrollmentResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public string ClassroomName { get; set; } = null!;
    public Enrollment.EnrollmentStatus Status { get; set; }
    public DateTime EnrolledAt { get; set; }
}

