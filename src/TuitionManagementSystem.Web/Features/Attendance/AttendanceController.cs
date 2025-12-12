using Microsoft.AspNetCore.Mvc;

namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
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
}
