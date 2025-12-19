namespace TuitionManagementSystem.Web.Features.Attendance.GenerateAttendanceCode;

using Ardalis.Result;
using MediatR;

public record GenerateAttendanceCodeRequest(int SessionId)
    : IRequest<Result<GenerateAttendanceCodeResponse>>;
