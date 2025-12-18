namespace TuitionManagementSystem.Web.Features.Homework;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;
using StudentHomework;
using TeacherHomework;

public class HomeworkController (IMediator mediator, ApplicationDbContext db) : Controller
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
        return this.View(result.Value);
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
        return this.View(result.Value);
    }



}
