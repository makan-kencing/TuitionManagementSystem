using Microsoft.AspNetCore.Mvc;

namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using AttendanceSummary;
using DeleteAttendance;
using GenerateAttendanceCode;
using MediatR;
using TakeAttendanceCode;

public class AttendanceController(IMediator mediator) : Controller
{

    [HttpPost("generate")]
    [TranslateResultToActionResult]
    public async Task<Result<GenerateAttendanceCodeResponse>> GenerateAttendanceCode(
        [FromForm] int sessionId,
        CancellationToken cancellationToken) => await mediator.Send(
        new GenerateAttendanceCodeRequest(
            sessionId),
        cancellationToken);

    [HttpPost("take")]
    [TranslateResultToActionResult]
    public async Task<Result<TakeAttendanceCodeResponse>>TakeAttendanceCode(
        [FromForm] string code,
        CancellationToken cancellationToken) => await mediator.Send(
        new TakeAttendanceCodeRequest(
            code),
        cancellationToken);

    [HttpDelete("delete/{attendanceId}")]
    [TranslateResultToActionResult]
    public async Task<Result<DeleteAttendanceResponse>> DeleteAttendance(
        [FromRoute] int attendanceId,
        CancellationToken cancellationToken) => await mediator.Send(
            new DeleteAttendanceRequest(attendanceId),cancellationToken);

    [HttpGet("summary/{studentId}")]
    [TranslateResultToActionResult]
    public async Task<Result<AttendanceSummaryViewModel>> AttendanceSummary(
        [FromRoute] int studentId,
        CancellationToken cancellationToken) => await mediator.Send(
        new AttendanceSummaryRequest(studentId),cancellationToken);
}
