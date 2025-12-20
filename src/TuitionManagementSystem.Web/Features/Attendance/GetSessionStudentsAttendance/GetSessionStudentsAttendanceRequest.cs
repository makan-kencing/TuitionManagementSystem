namespace TuitionManagementSystem.Web.Features.Attendance.GetSessionStudentsAttendance;

using Ardalis.Result;
using MediatR;

public record GetSessionStudentsAttendanceRequest(int SessionId)
    : IRequest<Result<GetSessionStudentsAttendanceResponse>>;
