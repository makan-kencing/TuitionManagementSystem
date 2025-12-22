using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MediatR;
using TuitionManagementSystem.Web.Features.Dashboard.StudentDashboard;
using TuitionManagementSystem.Web.Features.Dashboard.TeacherDashboard;
using TuitionManagementSystem.Web.Services.Auth.Constants;
using TuitionManagementSystem.Web.Services.Auth.Extensions;

namespace TuitionManagementSystem.Web.Features.Home;

using Dashboard.GetParentDashboard;
using Dashboard.TeacherDashboard;

public class HomeController(IMediator mediator) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (this.User.IsInRole(nameof(AccessRoles.Administrator)))
        {
            return this.RedirectToAction("AdminDashBoard", "Admin");
        }

        switch (this.User.GetUserType())
        {
            case nameof(Student):
            {
                var response = await mediator.Send(
                    new StudentDashboardRequest { StudentId = this.User.GetUserId() },
                    cancellationToken
                );

                return this.View("StudentDashboard", response);
            }

            case nameof(Teacher):
            {
                var response = await mediator.Send(
                    new TeacherDashboardRequest { TeacherId = this.User.GetUserId() },
                    cancellationToken
                );

                return this.View("TeacherDashboard", response);
            }

            case nameof(Parent):
            {
                var response = await mediator.Send(
                    new GetParentDashboardQuery(this.User.GetUserId()),
                    cancellationToken
                );

                return this.View("ParentDashboard", response.Value);
            }

            default:
                return this.View("GuestDashboard");
        }
    }

    [AllowAnonymous]
    [Route("404")]
    [IgnoreAntiforgeryToken]
    public IActionResult NotFoundPage()
    {
        this.Response.StatusCode = 404;
        return this.View("404");
    }

    [AllowAnonymous]
    [Route("500")]
    [IgnoreAntiforgeryToken]
    public IActionResult ServerError()
    {
        this.Response.StatusCode = 500;
        return this.View("500");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
