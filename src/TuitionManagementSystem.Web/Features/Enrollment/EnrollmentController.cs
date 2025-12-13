using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using MediatR;
using TuitionManagementSystem.Web.Features.Enrollment.MakeEnrollment;
using TuitionManagementSystem.Web.Models.ViewModels;
// using TuitionManagementSystem.Web.Features.Enrollment.ViewEnrollment;
// using TuitionManagementSystem.Web.Features.Enrollment.CancelEnrollment;

namespace TuitionManagementSystem.Web.Features.Enrollment;

[ApiController]
[Route("enrollment")]
public class EnrollmentController : Controller
{
    private readonly IMediator _mediator;

    public EnrollmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("make")]
    public async Task<IActionResult> MakeEnrollment(
        [FromForm] EnrollmentViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _mediator.Send(
            new MakeEnrollmentRequest(model.StudentId, model.CourseId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        return RedirectToAction("Index");
    }

    // [HttpGet("{id:int}")]
    // [TranslateResultToActionResult]
    // public async Task<Result<ViewEnrollmentResponse>> ViewEnrollment(
    //     int id,
    //     CancellationToken cancellationToken)
    // {
    //     return await _mediator.Send(
    //         new ViewEnrollmentRequest(id),
    //         cancellationToken);
    // }

    // [HttpPost("cancel")]
    // [TranslateResultToActionResult]
    // public async Task<Result> CancelEnrollment(
    //     [FromForm] int enrollmentId,
    //     CancellationToken cancellationToken)
    // {
    //     return await _mediator.Send(
    //         new CancelEnrollmentRequest(enrollmentId),
    //         cancellationToken);
    // }
}
