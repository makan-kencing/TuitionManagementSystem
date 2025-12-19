namespace TuitionManagementSystem.Web.Features.Homework;

using Ardalis.Result;
using GetAnnouncementInfo;
using Htmx;
using Infrastructure.Persistence;
using MakeAnnouncement;
using MakeSubmission;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Models.User;
using Services.Auth.Constants;
using Services.Auth.Extensions;
using StudentHomework;
using TeacherHomework;

public class HomeworkController(IMediator mediator, ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> HomeworkMenu(CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var result = await mediator.Send(new GetStudentHomeworkRequest(userId), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View("HomeworkMenu",result.Value);
    }

    public async Task<IActionResult> HomeworkManageMenu(CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var result = await mediator.Send(new GetTeacherHomeworkRequest(userId), cancellationToken);
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View("HomeworkManageMenu",result.Value);
    }

    [HttpGet]
    [Route("~/[controller]/make-submission/{id:int:required}")]
    public IActionResult GetMakeSubmissionModal(int id)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        return this.PartialView("_MakeSubmissionModel", new MakeSubmissionViewModel { AssignmentId = id });
    }

    [HttpGet]
    [Route("~/[controller]/make-announcement/{id:int:required}")]
    public IActionResult GetMakeAnnouncementModal(int id)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        return this.PartialView("_MakeAnnouncementModel", new MakeAnnouncementViewModel { CourseId = id });
    }

    [HttpPost]
    [Route("~/[controller]/submission")]
    public async Task<IActionResult> MakeSubmission(MakeSubmissionViewModel model)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }
        var response = await mediator.Send(new MakeSubmissionRequest(model.AssignmentId, userId,model.FileIds,model.Content));
        return this.PartialView("_SubmissionSuccess", model);
    }



    [HttpPost]
    [Route("~/[controller]/announcement")]
    public async Task<IActionResult> MakeAnnouncement(MakeAnnouncementViewModel model)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }
        var response = await mediator.Send(new MakeAnnouncementInfoRequest(model.CourseId,userId,model.FileIds,model.Title,model.Description,model.DueAt));
        return this.PartialView("_MakeAnnouncementSuccess", model);
    }


    public async Task<IActionResult> TeacherHomeworkDashboard(int courseId,CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }
        var response = await mediator.Send(new GetAnnouncementInfoRequest(courseId),cancellationToken);
        if (response.IsNotFound())
        {
            return this.NotFound();
        }
        return this.View(response.Value);
    }

    public async Task<IActionResult> StudentHomeworkDashboard(int courseId,CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }
        var response = await mediator.Send(new GetAnnouncementInfoRequest(courseId),cancellationToken);
        if (response.IsNotFound())
        {
            return this.NotFound();
        }
        return this.View(response.Value);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (User.IsInRole(nameof(AccessRoles.Administrator)))
        {
            return this.RedirectToAction("AdminDashBoard", "Admin");
        }

        switch (this.User.GetUserType())
        {
            case nameof(Student):
                return await this.HomeworkMenu(cancellationToken);
            case nameof(Teacher):
                return await this.HomeworkManageMenu(cancellationToken);
            default:
                return this.NotFound();
        }
    }


}
