namespace TuitionManagementSystem.Web.Features.Attendance;

using Abstractions;
using Ardalis.Result;
using DeleteAttendance;
using GenerateAttendanceCode;
using GetAttendanceCode;
using GetSessionStudentsAttendance;
using GetTeacherDailySessions;
using Htmx;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;
using TakeAttendanceCode;

public class SessionController(IMediator mediator) : WebController
{
    [HttpGet]
    [Route("~/[controller]/{id:int:required}/attendances")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> ViewStudentsAttendance(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSessionStudentsAttendanceRequest(id), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }

    [HttpGet]
    [Route("~/[controller]s/today")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> ViewDaily(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTeacherDailySessionsRequest(this.User.GetUserId()), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }

    [HttpPost]
    [Route("~/[controller]/{id:int:required}/attendance")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> GenerateAttendanceCodeHtmx(int id, CancellationToken cancellationToken)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var code = await mediator.Send(new GetAttendanceCodeQuery(id), cancellationToken);

        if (code.IsSuccess)
        {
            return this.PartialView("GenerateAttendanceCode/_AttendanceCodeGeneratedModal",
                new AttendanceCodeViewModel { SessionId = id, Code = code.Value.Code });
        }

        var generatedCode = await mediator.Send(
            new GenerateAttendanceCodeRequest(id),
            cancellationToken);

        if (generatedCode.IsForbidden())
        {
            return this.PartialView("GenerateAttendanceCode/_CannotGenerateAttendanceCodeModal");
        }

        return this.PartialView("GenerateAttendanceCode/_AttendanceCodeGeneratedModal",
            new AttendanceCodeViewModel { SessionId = id, Code = generatedCode.Value.Code });
    }

    [HttpPost]
    [Route("~/[controller]/{id:int:required}/student/{studentId:int:required}/attendance")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> MarkStudentAttendedHtmx(int id, int studentId, CancellationToken cancellationToken)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var code = await mediator.Send(new GetAttendanceCodeQuery(id), cancellationToken);
        if (code.IsError())
        {
            return this.NotFound("Helping take attendance code not function");
        }

        var takeAttendance =
            await mediator.Send(new TakeAttendanceCodeRequest(studentId, code.Value.Code), cancellationToken);
        if (takeAttendance.IsNotFound())
        {
            return this.NotFound("Helping take attendance code not function");
        }

        return this.PartialView("_MarkedStudentAttendanceModal", takeAttendance.Value);
    }

    [HttpDelete]
    [Route("~/[controller]/{id:int:required}/attendance/{attendanceId:int:required}")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> RemoveStudentAttendanceHtmx(int id, int attendanceId, CancellationToken cancellationToken)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var deleteAttendance =
            await mediator.Send(new DeleteAttendanceRequest(attendanceId), cancellationToken);
        if (deleteAttendance.IsError())
        {
            return this.BadRequest("Unable to delete attendance record.");
        }

        return this.PartialView("_DeleteAttendanceModal", deleteAttendance.Value);
    }

    public class AttendanceCodeViewModel
    {
        public int SessionId { get; init; }

        public required string Code { get; init; }
    }
}
