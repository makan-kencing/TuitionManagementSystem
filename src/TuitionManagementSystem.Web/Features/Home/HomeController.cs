namespace TuitionManagementSystem.Web.Features.Home;

using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.User;
using Services.Auth.Extensions;

public class HomeController(IMediator mediator) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
