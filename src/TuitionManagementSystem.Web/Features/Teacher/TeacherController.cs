namespace TuitionManagementSystem.Web.Features.Teacher;

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
[Route("admin/teachers")]
public class TeacherController(ApplicationDbContext db) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var teachers = await db.Teachers
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

        ViewData["Title"] = "Manage Teachers";
        ViewData["UserType"] = "Teacher";
        ViewData["CreateUrl"] = Url.Action(nameof(Create), "Teacher");

        return View("~/Views/Admin/UserList.cshtml", teachers);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add Teacher";
        return View("~/Views/Admin/CreateUser.cshtml", new AdminCreateUserViewModel());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        AdminCreateUserViewModel model,
        [FromServices] IMediator mediator)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Admin/CreateUser.cshtml", model);

        var result = await mediator.Send(new RegisterTeacherRequest
        {
            Username = model.Username,
            Email = model.Email,
            Password = model.Password,
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
        var teacher = await db.Teachers
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (teacher == null)
        {
            TempData["Status"] = "error";
            TempData["Message"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        teacher.Account.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        TempData["Status"] = "success";
        TempData["Message"] = "✅ Teacher deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}

