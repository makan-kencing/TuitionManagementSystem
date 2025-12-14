namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

public class AttendanceSummaryViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }

    public decimal OverallAttendanceRate { get; set; }

    public int TotalSessions { get; set; }

    public int AttendedSessions { get; set; }
    public List<CourseSummaryViewModel> Courses { get; set; }
}


