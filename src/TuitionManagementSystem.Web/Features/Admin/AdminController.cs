namespace TuitionManagementSystem.Web.Features.Admin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts;
using Authentication.Registration;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Auth.Constants;
using Services.File;
using User;

[Authorize(Roles = nameof(AccessRoles.Administrator))]
public class AdminController(ApplicationDbContext db,  IFileService fileService, IMediator mediator) : Controller
{
    public IActionResult AdminDashBoard() => this.View();

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
            ProfileImageUrl = account.ProfileImage?.Uri
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
}

