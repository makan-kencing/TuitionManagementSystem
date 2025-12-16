namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.User;

public sealed class RegisterParentRequestHandler(ApplicationDbContext db)
    : IRequestHandler<RegisterParentRequest, Result<RegisterParentResponse>>
{
    public async Task<Result<RegisterParentResponse>> Handle(
        RegisterParentRequest request,
        CancellationToken cancellationToken)
    {
        // Username uniqueness
        if (await db.Accounts.AnyAsync(
                a => a.Username == request.Username,
                cancellationToken))
        {
            return Result.Conflict("Username already exists");
        }

        // Email uniqueness
        if (await db.Accounts.AnyAsync(
                a => a.Email == request.Email,
                cancellationToken))
        {
            return Result.Conflict("Email already exists");
        }

        var account = new Account
        {
            Username = request.Username,
            Email = request.Email,
            HashedPassword = null
        };

        var hasher = new PasswordHasher<Account>();
        account.HashedPassword = hasher.HashPassword(account, request.Password);

        var parent = new Parent
        {
            Account = account
        };

        db.Parents.Add(parent);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new RegisterParentResponse
        {
            Id = parent.Id,           // 👈 inherited User.Id
            AccountId = account.Id
        });
    }
}
