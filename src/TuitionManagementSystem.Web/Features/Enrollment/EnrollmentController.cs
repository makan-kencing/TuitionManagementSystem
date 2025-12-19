using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using MediatR;
using TuitionManagementSystem.Web.Features.Enrollment.MakeEnrollment;
using TuitionManagementSystem.Web.Models.ViewModels;
using TuitionManagementSystem.Web.Features.Enrollment.ViewEnrollment;

namespace TuitionManagementSystem.Web.Features.Enrollment;

using Ardalis.Result.AspNetCore;
using MarkEnrollment;
using Microsoft.AspNetCore.Authorization;
using ViewCourseEnrollment;

[ApiController]
[Route("enrollment")]
public class EnrollmentController : Controller
{
    private readonly IMediator _mediator;

    public EnrollmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult MakeEnrollment() => this.View();

    [HttpPost("make")]
    public async Task<IActionResult> MakeEnrollment(
        [FromBody] EnrollmentViewModel model,
        CancellationToken cancellationToken)
    {
        if (!model.StudentId.HasValue || !model.CourseId.HasValue)
            return BadRequest(new { message = "Student and Course are required." });

        var result = await _mediator.Send(
            new MakeEnrollmentRequest(
                model.StudentId.Value,
                model.CourseId.Value),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        var response = new EnrollmentViewModel
        {
            EnrollmentId = result.Value.EnrollmentId,
            StudentId = result.Value.StudentId,
            CourseId = result.Value.CourseId,
            EnrolledAt = result.Value.EnrolledAt
        };

        return Ok(new { message = "Enrollment created successfully", enrollment = response });
    }

    [HttpGet("view/{id}")]
    public async Task<IActionResult> ViewEnrollment(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ViewEnrollmentRequest(id),
            cancellationToken);

        var model = result.IsSuccess
            ? result.Value
            : new List<ViewEnrollmentResponse>();

        if (!result.IsSuccess)
            ViewBag.Message = "No enrollments found for this student.";

        return PartialView("_ViewEnrollment", model);
    }


    [HttpPost("mark")]
    public async Task<IActionResult> MarkEnrollment(
        [FromBody] EnrollmentViewModel model,
        CancellationToken cancellationToken)
    {
        if (!model.EnrollmentId.HasValue)
            return BadRequest(new { error = "EnrollmentId is required" });

        if (!model.Status.HasValue)
            return BadRequest(new { error = "Status is required (Cancelled or Withdrawn)" });

        var result = await _mediator.Send(
            new MarkEnrollmentRequest { EnrollmentId = model.EnrollmentId.Value, Status = model.Status.Value },
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return Ok(new { message = $"Enrollment {model.Status.Value.ToString().ToLower()} successfully" });
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> ViewCourseEnrollments(int courseId, CancellationToken cancellationToken)
    {
        ViewBag.CourseId = courseId;
        return View("ViewCourseEnrollment");
    }

    [HttpGet("api/course/{courseId}/enrollments")]
    public async Task<IActionResult> GetCourseEnrollments(int courseId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ViewCourseEnrollmentsRequest(courseId),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        if (!result.Value.Any())
            return Ok(new { enrollments = new List<object>(), courseInfo = new object() });

        var firstEnrollment = result.Value.First();
        var courseInfo = new
        {
            CourseName = firstEnrollment.CourseName,
            SubjectName = firstEnrollment.SubjectName,
            CourseId = courseId
        };

        var enrollments = result.Value.Select(e => new
        {
            e.EnrollmentId,
            e.StudentId,
            e.StudentName,
            e.EnrolledAt,
            DaysSinceEnrolled = (DateTime.UtcNow - e.EnrolledAt).Days,
            e.AttendancePercentage,
            e.TotalSessions,
            e.AttendedSessions,
            CanCancel = (DateTime.UtcNow - e.EnrolledAt).Days < 14,
            CanWithdraw = (DateTime.UtcNow - e.EnrolledAt).Days >= 14
        });

        return Ok(new {
            enrollments,
            courseInfo,
            statistics = new {
                totalStudents = enrollments.Count(),
                averageAttendance = Math.Round(enrollments.Average(e => e.AttendancePercentage), 1),
                totalSessions = firstEnrollment.TotalSessions
            }
        });
    }
}

