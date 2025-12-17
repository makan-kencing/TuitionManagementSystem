    namespace TuitionManagementSystem.Web.Features.Accounts;

    using System.Security.Claims;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using TuitionManagementSystem.Web.Infrastructure.Persistence;
    using Models.User;
    using TuitionManagementSystem.Web.Services.Auth.Extensions;

    public sealed class ChangePasswordRequestHandler(ApplicationDbContext db, IHttpContextAccessor http)
        : IRequestHandler<ChangePasswordRequest, ChangePasswordResponse>
    {
        public async Task<ChangePasswordResponse> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = http.HttpContext?.User;
            if (user == null)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "User not authenticated."
                };
            }

            var userId = user.GetUserId();

            var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == userId, cancellationToken);
            if (account == null)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Account not found."
                };
            }

            var hasher = new PasswordHasher<Account>();

            var verify = hasher.VerifyHashedPassword(account, account.HashedPassword, request.CurrentPassword);
            if (verify == PasswordVerificationResult.Failed)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Current password is incorrect."
                };
            }

            if (request.NewPassword.Length < 6)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "New password must be at least 6 characters."
                };
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "New password and confirm password do not match."
                };
            }

            account.HashedPassword = hasher.HashPassword(account, request.NewPassword);
            await db.SaveChangesAsync(cancellationToken);

            return new ChangePasswordResponse
            {
                Success = true,
                Message = "Password changed successfully."
            };
        }
    }
