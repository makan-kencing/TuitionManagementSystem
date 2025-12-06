using Microsoft.AspNetCore.Mvc;

namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using GenerateAttendanceCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;

[Authorize]
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

}
