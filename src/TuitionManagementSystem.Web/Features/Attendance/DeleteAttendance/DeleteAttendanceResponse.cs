namespace TuitionManagementSystem.Web.Features.Attendance.DeleteAttendance;

public class DeleteAttendanceResponse
{
    public int UserId{ get; set; }
    public int SessionId{ get; set; }

    public required DateTime CurrentAt{ get; set; }

    public SessionTime? SessionTime { get; set; }
}

public class SessionTime
{
    public required DateTime StartAt{ get; set; }
    public required DateTime EndAt{ get; set; }

}
