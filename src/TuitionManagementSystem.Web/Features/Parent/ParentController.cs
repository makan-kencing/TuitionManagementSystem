namespace TuitionManagementSystem.Web.Features.Parent;

using System.Linq;
using System.Threading.Tasks;
using Admin;
using Authentication.Registration;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Constants;
using User;

[Authorize(Roles = nameof(AccessRoles.Administrator))]
[Route("admin/parents")]
public class ParentController(ApplicationDbContext db) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var parents = await db.Parents
            .Include(x => x.Account)
            .Where(x => x.Account.DeletedAt == null)
            .Select(x => new UserListViewModel
            {
                Id = x.Id,
                Username = x.Account.Username,
                DisplayName = x.Account.DisplayName,
                Email = x.Account.Email!
            })
            .ToListAsync();

        ViewData["Title"] = "Manage Parents";

        return View("~/Views/Admin/UserList.cshtml", parents);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add Parent";
        return View("~/Views/Admin/CreateUser.cshtml", new AdminCreateUserViewModel());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        AdminCreateUserViewModel model,
        [FromServices] IMediator mediator)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Admin/CreateUser.cshtml", model);

        var result = await mediator.Send(new RegisterParentRequest
        {
            Username = model.Username,
            Email = model.Email,
            Password = model.Password
        });

        TempData["Status"] = result.Value.IsSuccess ? "success" : "error";
        TempData["Message"] = result.Value.Message;

        if (result.Value.IsSuccess)
            return RedirectToAction(nameof(Index));

        return View("~/Views/Admin/CreateUser.cshtml", model);
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var parent = await db.Parents
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (parent == null)
        {
            TempData["Status"] = "error";
            TempData["Message"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        parent.Account.DeletedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        TempData["Status"] = "success";
        TempData["Message"] = "✅ User deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

}


