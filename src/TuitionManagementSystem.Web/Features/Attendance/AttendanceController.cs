using Microsoft.AspNetCore.Mvc;
using MediatR;
using TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using AttendanceHistory;
using Infrastructure.Persistence;
using Services.Auth.Extensions;

public class AttendanceController(IMediator mediator) : Controller
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
    public async Task<IActionResult> History(int courseId,CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId() ?? -1;
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var result = await mediator.Send(new GetAttendanceHistoryRequest(courseId,userId),cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }




}


