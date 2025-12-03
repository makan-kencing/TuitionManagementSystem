namespace TuitionManagementSystem.Web.Features.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class AdminController : Controller
{
    public IActionResult Index() => this.View();
}
