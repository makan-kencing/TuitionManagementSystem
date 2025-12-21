using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MediatR;
using TuitionManagementSystem.Web.Features.Dashboard.StudentDashboard;
using TuitionManagementSystem.Web.Services.Auth.Constants;
using TuitionManagementSystem.Web.Services.Auth.Extensions;

namespace TuitionManagementSystem.Web.Features.Home;

public class HomeController(IMediator mediator) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (User.IsInRole(nameof(AccessRoles.Administrator)))
        {
            return RedirectToAction("AdminDashBoard", "Admin");
        }

        switch (User.GetUserType())
        {
            case nameof(Student):
            {
                var response = await mediator.Send(
                    new StudentDashboardRequest
                    {
                        StudentId = User.GetUserId()
                    },
                    cancellationToken
                );

                return View("StudentDashboard", response);
            }

            case nameof(Teacher):
                return View("TeacherDashboard");

            case nameof(Parent):
                return View("ParentDashboard");

            default:
                return View("GuestDashboard");
        }
    }

    [AllowAnonymous]
    [Route("404")]
    [IgnoreAntiforgeryToken]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        return View("404");
    }

    [AllowAnonymous]
    [Route("500")]
    [IgnoreAntiforgeryToken]
    public IActionResult ServerError()
    {
        Response.StatusCode = 500;
        return View("500");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
}
