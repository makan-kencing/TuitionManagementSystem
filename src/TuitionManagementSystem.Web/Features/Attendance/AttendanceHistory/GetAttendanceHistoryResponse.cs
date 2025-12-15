namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceHistory;

public class GetAttendanceHistoryResponse
{
    public List<SessionAttendanceItem> Sessions { get; set; } = [];
}

public class SessionAttendanceItem
{
    public int SessionId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsValid { get; set; }
}
