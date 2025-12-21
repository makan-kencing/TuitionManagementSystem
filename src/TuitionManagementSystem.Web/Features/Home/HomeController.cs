using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace TuitionManagementSystem.Web.Features.Home;

using MediatR;
using Services.Auth.Constants;
using Services.Auth.Extensions;

public class HomeController(IMediator mediator) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(nameof(AccessRoles.Administrator)))
        {
            return RedirectToAction("AdminDashBoard", "Admin");
        }

        switch (this.User.GetUserType())
        {
            case nameof(Student):
                return this.View("StudentDashboard");
            case nameof(Teacher):
                return this.View("TeacherDashboard");
            case nameof(Parent):
                return this.View("ParentDashboard");
            default:
                return this.View("GuestDashboard");
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
        this.View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
}
