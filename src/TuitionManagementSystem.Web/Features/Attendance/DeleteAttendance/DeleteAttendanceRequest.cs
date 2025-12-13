namespace TuitionManagementSystem.Web.Features.Attendance.DeleteAttendance;

using Ardalis.Result;
using MediatR;

public record DeleteAttendanceRequest(int AttendanceId, int StudentId)
    : IRequest<Result<DeleteAttendanceResponse>>;

