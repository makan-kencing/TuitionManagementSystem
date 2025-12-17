namespace TuitionManagementSystem.Web.Features.Attendance.DeleteAttendance;

using Ardalis.Result;
using MediatR;

public record DeleteAttendanceRequest(int AttendanceId ,int UserId,int SessionId) : IRequest<Result<DeleteAttendanceResponse>>;

