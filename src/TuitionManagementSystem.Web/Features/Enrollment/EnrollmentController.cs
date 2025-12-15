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

        // Send request to handler
        var result = await _mediator.Send(
            new MakeEnrollmentRequest(
                model.StudentId.Value,
                model.CourseId.Value),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        // Return structured response
        var response = new EnrollmentViewModel
        {
            EnrollmentId = result.Value.EnrollmentId,
            StudentId = result.Value.StudentId,
            CourseId = result.Value.CourseId,
            EnrolledAt = result.Value.EnrolledAt
        };

        return Ok(new
        {
            message = "Enrollment created successfully",
            enrollment = response
        });
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
            new MarkEnrollmentRequest
            {
                EnrollmentId = model.EnrollmentId.Value,
                Status = model.Status.Value
            },
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return Ok(new {
            message = $"Enrollment {model.Status.Value.ToString().ToLower()} successfully"
        });
    }
}

