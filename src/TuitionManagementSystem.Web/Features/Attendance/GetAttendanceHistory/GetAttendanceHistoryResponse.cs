namespace TuitionManagementSystem.Web.Features.Attendance.GetAttendanceHistory;

public class GetAttendanceHistoryResponse
{
    public required string CourseName { get; set; }

    public DateTime EnrollmentAt { get; set; }
    public List<SessionAttendanceItem> Sessions { get; set; } = [];
}

public class SessionAttendanceItem
{
    public int SessionId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsValid { get; set; }
}
