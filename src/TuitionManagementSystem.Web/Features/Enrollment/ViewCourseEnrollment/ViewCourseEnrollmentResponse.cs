namespace TuitionManagementSystem.Web.Features.Enrollment.ViewCourseEnrollment;

public class ViewCourseEnrollmentsResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int TotalSessions { get; set; }
    public int AttendedSessions { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
}
