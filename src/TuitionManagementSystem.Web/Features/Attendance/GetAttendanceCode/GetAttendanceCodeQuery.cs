namespace TuitionManagementSystem.Web.Features.Attendance.GetAttendanceCode;

using Ardalis.Result;
using MediatR;

public record GetAttendanceCodeQuery(int SessionId) : IRequest<Result<GetAttendanceCodeResponse>>;
