namespace TuitionManagementSystem.Web.Features.Attendance.DeleteAttendance;

public class DeleteAttendanceResponse
{
    public required int StudentId { get; init; }

    public required int SessionId { get; init; }

    public required SessionTime SessionTime { get; init; }
}

public class SessionTime
{
    public required DateTime StartAt { get; init; }

    public required DateTime EndAt { get; init; }
}
