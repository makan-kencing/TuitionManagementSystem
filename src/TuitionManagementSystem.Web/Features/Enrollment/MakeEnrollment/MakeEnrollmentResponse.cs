namespace TuitionManagementSystem.Web.Features.Enrollment.MakeEnrollment;

public class MakeEnrollmentResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
}
