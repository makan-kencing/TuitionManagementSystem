namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using AttendanceHistory;
using AttendanceSummary;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;
using TeacherDailySessionList;

public class AttendanceController(IMediator mediator, ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Summary(
        CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId() ?? -1;
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
        var userId = this.User.GetUserId() ?? -1;
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

    [Authorize(Policy =  "TeacherOnly")]
    public async Task<IActionResult> GenerateAttendance( CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId() ?? -1;
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var result =await mediator.Send(new GetTeacherDailySessionListRequest(userId!), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);

    }
}
