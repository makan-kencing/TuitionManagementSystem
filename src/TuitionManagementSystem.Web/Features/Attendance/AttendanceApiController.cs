using Microsoft.AspNetCore.Mvc;

namespace TuitionManagementSystem.Web.Features.Attendance;

using Abstractions;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using AttendanceSummary;
using DeleteAttendance;
using GenerateAttendanceCode;
using MediatR;
using Services.Auth.Extensions;
using TakeAttendanceCode;


public class AttendanceApiController(IMediator mediator) : ApiController
{
    [HttpPost("generate")]
    public async Task<Result<GenerateAttendanceCodeResponse>> GenerateAttendanceCode(
        [FromForm] int sessionId,
        CancellationToken cancellationToken) =>
        await mediator.Send(new GenerateAttendanceCodeRequest(sessionId), cancellationToken);

    [HttpPost("take")]
    public async Task<Result<TakeAttendanceCodeResponse>> TakeAttendanceCode(
        [FromForm] string code,
        CancellationToken cancellationToken) =>
        await mediator.Send(new TakeAttendanceCodeRequest(this.User.GetUserId(), code), cancellationToken);

    [HttpDelete("delete")]
    public async Task<Result> DeleteAttendance(
        [FromForm] int attendanceId,
        CancellationToken cancellationToken) =>
        await mediator.Send(new DeleteAttendanceRequest(attendanceId), cancellationToken);

   /* [HttpGet("summary/{studentId}")]
    public async Task<Result<GetAttendanceSummaryResponse>> AttendanceSummary(
        [FromRoute] int studentId,
        CancellationToken cancellationToken) =>
        await mediator.Send(new GetAttendanceSummaryRequest(studentId), cancellationToken);
        */


}
