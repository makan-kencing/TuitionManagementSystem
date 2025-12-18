namespace TuitionManagementSystem.Web.Features.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Constants;

[Authorize(Roles = nameof(AccessRoles.Administrator))]
public class AdminController : Controller
{
    public IActionResult AdminDashBoard() => this.View();

    public IActionResult AdminPanel() => this.View();
}
