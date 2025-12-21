namespace TuitionManagementSystem.Web.Features.Homework;

using Ardalis.Result;
using GetAnnouncementDetail;
using GetAnnouncementInfo;
using GetAssignmentDetail;
using GetSubmissionFile;
using Htmx;
using Infrastructure.Persistence;
using MakeAnnouncement;
using MakeSubmission;
using MarkHomework;
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

        return this.View("HomeworkMenu", result.Value);
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

        return this.View("HomeworkManageMenu", result.Value);
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

        var response =
            await mediator.Send(new MakeSubmissionRequest(model.AssignmentId, userId, model.FileIds, model.Content));
        return this.PartialView("_SubmissionSuccess", model);
    }


    [HttpPost]
    [Authorize(Policy = "TeacherOnly")]
    [Route("~/[controller]/announcement")]
    public async Task<IActionResult> MakeAnnouncement(MakeAnnouncementViewModel model)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }
        if (string.IsNullOrWhiteSpace(model.Title))
        {
                return this.PartialView("_MakeAnnouncementError", model);
        }


        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var response = await mediator.Send(new MakeAnnouncementInfoRequest(model.CourseId, userId, model.FileIds,
            model.Title, model.Description, model.DueAt));



        return this.PartialView("_MakeAnnouncementSuccess", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetSubmissionFile(int id, CancellationToken cancellationToken)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var response = await mediator.Send(new GetSubmissionFileRequest(id), cancellationToken);
        var value = response.Value;

        return this.PartialView("_MarkHomeWorkModel",
            new MarkHomeworkViewModel
            {
                Student = new HomeworkStudentInfo { Name = value.Name },
                Submission = new HomeworkSubmissionInfo { Id = value.Id, Content = null },
                SubmissionFiles = value.SubmissionFiles
            });
    }

    [HttpPut]
    [Route("~/[controller]/{id:int:required}/mark")]
    public async Task<IActionResult> MarkHomework(int id, int grade, CancellationToken cancellationToken)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        await mediator.Send(new MarkHomeworkRequest(id, grade), cancellationToken);

        this.Response.Htmx(h => h.Refresh());
        return this.Ok();
    }

    public class MarkHomeworkSuccessViewModel
    {
        public required int SubmissionId { get; set; }
    }

    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> TeacherHomeworkDashboard(int courseId, CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var response = await mediator.Send(new GetAnnouncementInfoRequest(courseId), cancellationToken);
        if (response.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(response.Value);
    }

    public async Task<IActionResult> StudentHomeworkDashboard(int courseId, CancellationToken cancellationToken)
    {
        var userId = this.User.GetUserId();
        if (userId == -1)
        {
            return this.Unauthorized();
        }

        var response = await mediator.Send(new GetAnnouncementInfoRequest(courseId), cancellationToken);
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

    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> SubmissionList(int courseId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAnnouncementInfoRequest(courseId), cancellationToken);
        if (response.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(response.Value);
    }

    // public IActionResult AssignmentDetail()=>this.View();
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> GetAssignmentDetail(int assignmentId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAssignmentDetailsQuery(assignmentId), cancellationToken);
        if (response.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(response.Value);
    }

    public async Task<IActionResult> AnnouncementDetail(int announcementId, CancellationToken cancellationToken)
    {

        var userId = this.User.GetUserId();

        var response = await mediator.Send(new GetAnnouncementDetailRequest(announcementId, userId), cancellationToken);
        return this.View(response.Value);
    }
}
