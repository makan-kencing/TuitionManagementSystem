namespace TuitionManagementSystem.Web.Features.Attendance.GenerateAttendanceCode;

using MediatR;

public class GenerateAttendanceCodeRequest : IRequest<GenerateAttendanceCodeResponse>
{
    public int SessionId { get; set; }
}



