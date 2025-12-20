namespace TuitionManagementSystem.Web.Features.Attendance;

using Ardalis.Result;
using DeleteAttendance;
using GenerateAttendanceCode;
using GetAttendanceCode;
using GetAttendanceHistory;
using GetAttendanceSummary;
using GetSessionStudentsAttendance;
using GetTeacherDailySessions;
using Htmx;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;
using TakeAttendanceCode;

public class AttendanceController(IMediator mediator) : Controller
{
    [HttpGet]
    [Route("~/[controller]s")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAttendanceSummaryRequest(this.User.GetUserId()), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View("Summary", result.Value);
    }

    [HttpGet]
    [Route("~/[controller]/{id:int:required}")]
    public async Task<IActionResult> CourseHistory(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAttendanceHistoryRequest(id, this.User.GetUserId()), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(result.Value);
    }

    [HttpGet]
    [Route("~/[controller]/take")]
    public IActionResult TakeAttendance() => this.View();

    [HttpPost]
    [Route("~/[controller]/take")]
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
}
