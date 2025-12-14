using Microsoft.AspNetCore.Mvc;
using MediatR;
using TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

namespace TuitionManagementSystem.Web.Features.Attendance;

public class AttendanceController(IMediator mediator) : Controller
{
    [HttpGet("student/attendance")]
    public async Task<IActionResult> StudentAttendance(
        CancellationToken cancellationToken)
    {
        var studentIdClaim = User.FindFirst("StudentId");

        if (studentIdClaim == null)
        {
            return Unauthorized();
        }

        var studentId = int.Parse(studentIdClaim.Value);

        var result = await mediator.Send(
            new AttendanceSummaryRequest(studentId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return View(result.Value);
    }
}
