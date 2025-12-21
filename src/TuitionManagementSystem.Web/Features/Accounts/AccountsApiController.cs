namespace TuitionManagementSystem.Web.Features.Accounts;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.User;
using Services.Auth.Extensions;
using Services.File;

[AllowAnonymous]
public sealed class AccountsApiController(ApplicationDbContext db, IMediator mediator, IFileService fileService) : ApiController
{
    private static readonly PasswordHasher<Account> Hasher = new();

    [HttpGet("check/username")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<bool> CheckUsernameUnique([FromForm] string username, CancellationToken cancellationToken) =>
        !await db.Accounts
            .Where(a => a.Username == username)
            .AnyAsync(cancellationToken);

    [HttpGet("check/email")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<bool> CheckEmailUnique([FromForm] string email, CancellationToken cancellationToken) =>
        !await db.Accounts
            .Where(a => a.Email == email)
            .AnyAsync(cancellationToken);

    [HttpPost]
    [Route("profile")]
    public async Task<object> UpdateProfile([FromForm] AccountProfileViewModel model)
    {
        var userId = this.User.GetAccountId();
        var user = await db.Accounts
            .Include(a => a.ProfileImage)
            .FirstOrDefaultAsync(a => a.Id == userId);

        if (user == null)
            return new { success = false, message = "User not found." };

        // Verify current password
        if (model.ConfirmPassword != null)
        {
            var pwdCheck = Hasher.VerifyHashedPassword(user, user.HashedPassword, model.ConfirmPassword);
            if (pwdCheck == PasswordVerificationResult.Failed)
                return new { success = false, message = "Current password is incorrect." };
        }

        // Update username if changed
        if (!string.Equals(user.Username, model.Username, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await db.Accounts.AnyAsync(a => a.Username == model.Username && a.Id != user.Id);
            if (exists)
                return new { success = false, message = "Username is already taken." };

            user.Username = model.Username;
        }

        // Update displayname if changed
        if (!string.Equals(user.DisplayName, model.DisplayName, StringComparison.OrdinalIgnoreCase))
        {
            user.DisplayName = model.DisplayName;
        }

        // Update email if changed
        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await db.Accounts.AnyAsync(a => a.Email == model.Email && a.Id != user.Id);
            if (emailExists)
                return new { success = false, message = "Email is already in use." };

            user.Email = model.Email;
        }

        if (user.IsTwoFactorEnabled != model.IsTwoFactorEnabled)
        {
            user.IsTwoFactorEnabled = model.IsTwoFactorEnabled;

            // Clear any existing token when disabling
            if (!model.IsTwoFactorEnabled)
            {
                user.TwoFactorToken = null;
                user.TwoFactorTokenExpiry = null;
            }
        }

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var oldImage = user.ProfileImage;

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

            user.ProfileImageId = newFile.Id;

            await db.SaveChangesAsync();

            if (oldImage != null)
            {
                if (!string.IsNullOrWhiteSpace(oldImage.CanonicalPath))
                {
                    await fileService.DeleteFileAsync(oldImage.CanonicalPath);
                }

                db.Files.Remove(oldImage);
                await db.SaveChangesAsync();
            }
        }

        user.LastChanged = DateTime.UtcNow;

        db.Accounts.Update(user);
        await db.SaveChangesAsync();

        return new { success = true, message = "Profile updated successfully." };
    }

    [HttpPost]
    [Route("password")]
    public async Task<object> ChangePassword([FromForm]ChangePasswordRequest request)
    {
        var result = await mediator.Send(request);
        return new { success = result.Success, message = result.Message };
    }
}
