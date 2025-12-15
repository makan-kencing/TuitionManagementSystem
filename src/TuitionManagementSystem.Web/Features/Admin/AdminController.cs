namespace TuitionManagementSystem.Web.Features.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Constants;

[Authorize(Roles = nameof(AccessRoles.Administrator))]
public class AdminController : Controller
{
    public IActionResult Index() => this.View();
}
