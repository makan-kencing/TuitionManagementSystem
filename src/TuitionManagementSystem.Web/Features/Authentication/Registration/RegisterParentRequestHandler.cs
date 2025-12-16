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
        var errors = new List<string>();

        // Check for duplicate username
        if (await db.Accounts.AnyAsync(a => a.Username == request.Username, cancellationToken))
            errors.Add("⚠ Username already exists!");

        // Check for duplicate email
        if (await db.Accounts.AnyAsync(a => a.Email == request.Email, cancellationToken))
            errors.Add("⚠ Email already exists!");

        // If there are errors, return them all in one response
        if (errors.Any())
        {
            return Result.Success(new RegisterParentResponse
            {
                IsSuccess = false,
                Message = string.Join("<br/>", errors) // join with line breaks for SweetAlert
            });
        }

        // Hash password
        var hasher = new PasswordHasher<Account>();
        var hashedPassword = hasher.HashPassword(null, request.Password);

        // Create account
        var account = new Account
        {
            Username = request.Username,
            DisplayName = request.Username,
            Email = request.Email,
            HashedPassword = hashedPassword
        };

        // Create parent
        var parent = new Parent { Account = account };

        db.Parents.Add(parent);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new RegisterParentResponse
        {
            Id = parent.Id,
            AccountId = account.Id,
            IsSuccess = true,
            Message = "✅ Successfully added a new parent account!"
        });
    }
}

