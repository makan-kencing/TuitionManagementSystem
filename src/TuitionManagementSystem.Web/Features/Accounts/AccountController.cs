namespace TuitionManagementSystem.Web.Features.Accounts;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Services.Auth.Extensions;

public class AccountController(ApplicationDbContext db, IMediator _mediator) : Controller
{
    public IActionResult AccountProfile() => this.View();

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(AccountProfileViewModel model)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "User not authenticated." });

        var user = await db.Accounts.FirstOrDefaultAsync(a => a.Id == userId.Value);
        if (user == null)
            return Json(new { success = false, message = "User not found." });

        var hasher = new PasswordHasher<Account>();
        var pwdCheck = hasher.VerifyHashedPassword(user, user.HashedPassword, model.ConfirmPassword);
        if (pwdCheck == PasswordVerificationResult.Failed)
            return Json(new { success = false, message = "Current password is incorrect." });

        if (!string.Equals(user.Username, model.Username, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await db.Accounts.AnyAsync(a => a.Username == model.Username);
            if (exists)
                return Json(new { success = false, message = "Username is already taken." });

            user.Username = model.Username;
        }

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await db.Accounts.AnyAsync(a => a.Email == model.Email);
            if (emailExists)
                return Json(new { success = false, message = "Email is already in use." });

            user.Email = model.Email;
        }

        await db.SaveChangesAsync();

        return Json(new { success = true, message = "Profile updated successfully." });
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var result = await _mediator.Send(request);
        return Json(new { success = result.Success, message = result.Message });
    }

}
