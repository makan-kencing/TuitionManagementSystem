namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using AttendanceHistory;
using AttendanceSummary;
using GenerateAttendanceCode;
using GetAttendanceCode;
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

    // public IActionResult GenerateAttendance() => this.View();

    [HttpGet]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> GenerateAttendance(CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var result = await mediator.Send(new GetTeacherDailySessionListRequest(userId!), cancellationToken);
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
            return this.PartialView("_AttendanceCodeModal", new AttendanceCodeViewModel
            {
                SessionId = id,
                Code = code.Value.Code
            });
        }

        var generatedCode = await mediator.Send(
            new GenerateAttendanceCodeRequest(id),
            cancellationToken);

        return this.PartialView("_AttendanceCodeModal",
            new AttendanceCodeViewModel
            {
                SessionId = id,
                Code = generatedCode.Value.Code
            });
    }




    // public IActionResult CourseSessionListing() => this.View();
    [HttpGet]
    public async Task<IActionResult> CourseSessionListing(int sessionId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSessionStudentListRequest(sessionId), cancellationToken);
        return this.View(result.Value);
    }

    [HttpPost]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> Take(int id,int userId, CancellationToken cancellationToken)
    {
        var code = await mediator.Send(new GetAttendanceCodeQuery(id), cancellationToken);
        if (code.IsError())
        {
            return this.NotFound("Helping take attendance code not function");
        }

        var takeAttendance =
            await mediator.Send(new TakeAttendanceCodeRequest(userId, code.Value.Code), cancellationToken);
        if (takeAttendance.IsNotFound())
        {
            return this.NotFound("Helping take attendance code not function");
        }
        return this.PartialView("_AttendanceManagerModel",takeAttendance.Value);
    }

}
