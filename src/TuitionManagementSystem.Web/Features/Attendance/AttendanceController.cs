namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using AttendanceHistory;
using AttendanceSummary;
using DeleteAttendance;
using GenerateAttendanceCode;
using GetAttendanceCode;
using Htmx;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;
using SessionStudentList;
using TakeAttendanceCode;
using TeacherDailySessionList;

public class AttendanceController(IMediator mediator, ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Summary(
        CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var result = await mediator.Send(new GetAttendanceSummaryRequest(userId!), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> History(int courseId, CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var result = await mediator.Send(new GetAttendanceHistoryRequest(courseId, userId), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }

    [HttpGet]
    public IActionResult TakeAttendance() => this.View();

    [HttpGet]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> GenerateAttendance(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTeacherDailySessionListRequest(this.User.GetUserId()), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }

    [HttpPost]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> Generate(int id, CancellationToken cancellationToken)
    {
        var code = await mediator.Send(new GetAttendanceCodeQuery(id), cancellationToken);

        if (code.IsSuccess)
        {
            return this.PartialView("_AttendanceCodeModal",
                new AttendanceCodeViewModel { SessionId = id, Code = code.Value.Code });
        }

        var generatedCode = await mediator.Send(
            new GenerateAttendanceCodeRequest(id),
            cancellationToken);

        if (generatedCode.IsForbidden())
        {
            return this.PartialView("_CannotGenerateAttendanceCode");
        }

        return this.PartialView("_AttendanceCodeModal",
            new AttendanceCodeViewModel { SessionId = id, Code = generatedCode.Value.Code });
    }

    [HttpGet]
    public async Task<IActionResult> CourseSessionListing(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSessionStudentListRequest(id), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> TakeAttendanceHtmx([FromForm] string code)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var result = await mediator.Send(new TakeAttendanceCodeRequest(this.User.GetUserId(), code));
        if (result.IsNotFound())
        {
            return this.PartialView("TakeAttendance/_CodeNotFoundModal");
        }

        if (result.IsForbidden())
        {
            foreach (var error in result.ValidationErrors)
            {
                switch (error.ErrorMessage)
                {
                    case TakeAttendanceValidationError.NotEnrolled:
                        return this.PartialView("TakeAttendance/_NotEnrolledModal");
                    case TakeAttendanceValidationError.OutsideOfTime:
                        return this.PartialView("TakeAttendance/_OutsideClassTimeModal");
                }
            }
        }

        if (result.IsConflict())
        {
            return this.PartialView("TakeAttendance/_DuplicateAttendanceModal");
        }

        return this.PartialView("TakeAttendance/_AttendanceSuccessModal");
    }

    [HttpPost]
    [Route("~/[controller]/session/{sessionId:int:required}/student{studentId:int:required}")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> MarkStudentAttended(int sessionId, int studentId, CancellationToken cancellationToken)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var code = await mediator.Send(new GetAttendanceCodeQuery(sessionId), cancellationToken);
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
    [Route("~/[controller]/{id:int:required}")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> RemoveStudentAttendance(int id, CancellationToken cancellationToken)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var deleteAttendance =
            await mediator.Send(new DeleteAttendanceRequest(id), cancellationToken);
        if (deleteAttendance.IsError())
        {
            return this.BadRequest("Unable to delete attendance record.");
        }

        return this.PartialView("_DeleteAttendanceModal", deleteAttendance.Value);
    }
}
