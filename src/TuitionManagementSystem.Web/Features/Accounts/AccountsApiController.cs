namespace TuitionManagementSystem.Web.Features.Accounts;

using Abstractions;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Services.Auth.Extensions;

[AllowAnonymous]
public sealed class AccountsApiController(ApplicationDbContext db, IMediator mediator) : ApiController
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
    public async Task<object> UpdateProfile(AccountProfileViewModel model)
    {
        var userId = this.User.GetUserId();
        if (userId == null)
        {
            return new { success = false, message = "User not authenticated." };
        }

        var user = await db.Accounts.FirstOrDefaultAsync(a => a.Id == userId.Value);
        if (user == null)
        {
            return new { success = false, message = "User not found." };
        }

        var pwdCheck = Hasher.VerifyHashedPassword(user, user.HashedPassword, model.ConfirmPassword);
        if (pwdCheck == PasswordVerificationResult.Failed)
            return new { success = false, message = "Current password is incorrect." };

        if (!string.Equals(user.Username, model.Username, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await db.Accounts.AnyAsync(a => a.Username == model.Username);
            if (exists)
                return new { success = false, message = "Username is already taken." };

            user.Username = model.Username;
        }

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await db.Accounts.AnyAsync(a => a.Email == model.Email);
            if (emailExists)
                return new { success = false, message = "Email is already in use." };

            user.Email = model.Email;
        }

        await db.SaveChangesAsync();

        return new { success = true, message = "Profile updated successfully." };
    }

    [HttpPost]
    public async Task<object> ChangePassword(ChangePasswordRequest request)
    {
        var result = await mediator.Send(request);
        return new { success = result.Success, message = result.Message };
    }
}
