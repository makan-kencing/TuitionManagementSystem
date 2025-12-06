namespace TuitionManagementSystem.Web.Features.Attendance.GenerateAttendanceCode;
using Ardalis.Result;
using MediatR;


public record GenerateAttendanceCodeRequest(string SessionId)
    : IRequest<Result<GenerateAttendanceCodeResponse>>;
