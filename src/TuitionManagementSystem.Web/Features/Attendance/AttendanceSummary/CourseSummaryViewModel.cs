namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

public class CourseSummaryViewModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }

    public decimal AttendanceRate { get; set; }
    public int TotalSessions { get; set; }
    public int AttendedSessions { get; set; }
}
