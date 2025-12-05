namespace TuitionManagementSystem.Web.Features.Attendance.GenerateAttendanceCode;

public class GenerateAttendanceCodeResponse
{
    public int SessionId { get; set; }
    public string Code { get; set; } = default!;
    public DateOnly DatedFor { get; set; }
}


