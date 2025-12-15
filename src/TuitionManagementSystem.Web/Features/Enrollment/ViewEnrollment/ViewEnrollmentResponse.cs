namespace TuitionManagementSystem.Web.Features.Enrollment.ViewEnrollment;

public class ViewEnrollmentResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public string ClassroomName { get; set; } = null!;
    public DateTime EnrolledAt { get; set; }
}

