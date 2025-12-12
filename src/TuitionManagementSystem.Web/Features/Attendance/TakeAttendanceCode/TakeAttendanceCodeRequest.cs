namespace TuitionManagementSystem.Web.Features.Attendance.TakeAttendanceCode;
using Ardalis.Result;
using MediatR;
public record TakeAttendanceCodeRequest(string Code)
    : IRequest<Result<TakeAttendanceCodeResponse>>;
