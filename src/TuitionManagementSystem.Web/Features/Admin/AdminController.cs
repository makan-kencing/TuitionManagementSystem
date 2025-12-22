namespace TuitionManagementSystem.Web.Features.Admin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts;
using Authentication.Registration;
using Dashboard.TeacherDashboard;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.User;
using Services.Auth.Constants;
using Services.Auth.Extensions;
using Services.File;
using User;

[Authorize(Roles = nameof(AccessRoles.Administrator))]
public class AdminController(ApplicationDbContext db,  IFileService fileService, IMediator mediator) : Controller
{
    public async Task<IActionResult> AdminDashBoard(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new TeacherDashboardRequest { TeacherId = User.GetUserId() },
            cancellationToken
        );

       return this.View(response);
    }

    public IActionResult AdminPanel() => this.View();

    public async Task<IActionResult> EditUser(int userId)
    {
        var user = await db.Users
            .Include(u => u.Account)
            .ThenInclude(a => a.ProfileImage)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Account == null)
            return NotFound();

        var account = user.Account;

        var model = new UserProfileViewModel()
        {
            UserId = user.Id,
            Username = account.Username,
            DisplayName = account.DisplayName,
            Email = account.Email,
            ProfileImageUrl = account.ProfileImage != null
                ? account.ProfileImage.Uri
                : "/assets/img/DefaultProfile.png"
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser([FromForm] UserProfileViewModel model)
    {
        var errors = new System.Collections.Generic.List<string>();

        if (!ModelState.IsValid)
        {
            errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { success = false, errors });
        }

        var user = await db.Users
            .Include(u => u.Account)
            .ThenInclude(a => a.ProfileImage)
            .FirstOrDefaultAsync(u => u.Id == model.UserId);

        if (user == null || user.Account == null)
            return Json(new { success = false, errors = new[] { "User not found." } });

        var account = user.Account;

        // Update username
        if (!string.Equals(account.Username, model.Username, StringComparison.OrdinalIgnoreCase))
        {
            if (await db.Accounts.AnyAsync(a => a.Username == model.Username && a.Id != account.Id))
                return Json(new { success = false, errors = new[] { "Username is already taken." } });
            account.Username = model.Username;
        }

        account.DisplayName = model.DisplayName;

        // Update email
        if (!string.Equals(account.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await db.Accounts.AnyAsync(a => a.Email == model.Email && a.Id != account.Id))
                return Json(new { success = false, errors = new[] { "Email is already in use." } });
            account.Email = model.Email;
        }

        string newProfileImageUrl = account.ProfileImage?.Uri;

        // Update profile image
        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var oldImage = account.ProfileImage;
            var savedFile = await fileService.UploadFileAsync(model.ProfileImage);

            var newFile = new File
            {
                FileName = model.ProfileImage.FileName,
                MimeType = model.ProfileImage.ContentType,
                Uri = savedFile.MappedPath,
                CanonicalPath = savedFile.CanonicalPath
            };

            db.Files.Add(newFile);
            await db.SaveChangesAsync();

            account.ProfileImageId = newFile.Id;
            await db.SaveChangesAsync();

            if (oldImage != null)
            {
                if (!string.IsNullOrWhiteSpace(oldImage.CanonicalPath))
                    await fileService.DeleteFileAsync(oldImage.CanonicalPath);

                db.Files.Remove(oldImage);
                await db.SaveChangesAsync();
            }

            newProfileImageUrl = newFile.Uri;
        }

        account.LastChanged = DateTime.UtcNow;
        db.Accounts.Update(account);
        await db.SaveChangesAsync();

        return Json(new { success = true, message = "Profile updated successfully.", newProfileImageUrl });
    }

    //Sorting
    public async Task<IActionResult> UserList(
        string? sortColumn,
        string? sortOrder,
        string? search,
        string? userType)
    {
        ViewData["UserType"] = userType;

        ViewData["CreateUrl"] = userType switch
        {
            "Teacher" => Url.Action("Create", "Teacher"),
            "Student" => Url.Action("Create", "Student"),
            "Parent"  => Url.Action("Create", "Parent"),
            _ => null
        };

        sortColumn ??= "Id";
        sortOrder ??= "asc";

        IQueryable<User?> query = userType switch
        {
            "Teacher" => db.Teachers
                .Include(t => t.Account)
                .Where(t => t.Account.DeletedAt == null)
                .Select(t => t.Account.User),

            "Student" => db.Students
                .Include(s => s.Account)
                .Where(s => s.Account.DeletedAt == null)
                .Select(s => s.Account.User),

            "Parent" => db.Parents
                .Include(p => p.Account)
                .Where(p => p.Account.DeletedAt == null)
                .Select(p => p.Account.User),

            _ => db.Users
                .Include(u => u.Account)
                .Where(u => u.Account.DeletedAt == null)
        };


        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                u.Account.Username.Contains(search) ||
                u.Account.DisplayName.Contains(search) ||
                u.Account.Email.Contains(search));
        }

        query = (sortColumn, sortOrder.ToLower()) switch
        {
            ("Username", "asc") => query.OrderBy(u => u.Account.Username),
            ("Username", "desc") => query.OrderByDescending(u => u.Account.Username),
            ("Name", "asc") => query.OrderBy(u => u.Account.DisplayName),
            ("Name", "desc") => query.OrderByDescending(u => u.Account.DisplayName),
            ("Email", "asc") => query.OrderBy(u => u.Account.Email),
            ("Email", "desc") => query.OrderByDescending(u => u.Account.Email),
            _ => query.OrderBy(u => u.Id)
        };

        var users = await query
            .Select(u => new UserListViewModel
            {
                Id = u.Id,
                Username = u.Account.Username,
                DisplayName = u.Account.DisplayName,
                Email = u.Account.Email!
            })
            .ToListAsync();

        ViewData["SortColumn"] = sortColumn;
        ViewData["SortOrder"] = sortOrder;
        ViewData["Search"] = search;

        // Return partial
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_UserListTableRows", users);
        }

        return View("~/Views/Admin/UserList.cshtml", users);
    }



}

