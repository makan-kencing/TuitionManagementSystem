namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Services.Auth.Constants;

public sealed class RegisterTeacherRequestHandler(ApplicationDbContext db)
    : IRequestHandler<RegisterTeacherRequest, Result<RegisterTeacherResponse>>
{
    public async Task<Result<RegisterTeacherResponse>> Handle(
        RegisterTeacherRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // Check duplicate username
        if (await db.Accounts.AnyAsync(a => a.Username == request.Username, cancellationToken))
            errors.Add("⚠ Username already exists!");

        // Check duplicate email
        if (await db.Accounts.AnyAsync(a => a.Email == request.Email, cancellationToken))
            errors.Add("⚠ Email already exists!");

        // Return all validation errors together
        if (errors.Any())
        {
            return Result.Success(new RegisterTeacherResponse
            {
                IsSuccess = false,
                Message = string.Join("<br/>", errors)
            });
        }

        // Hash password
        var hasher = new PasswordHasher<Account>();
        var hashedPassword = hasher.HashPassword(null!, request.Password);

        // Create account
        var account = new Account
        {
            Username = request.Username,
            DisplayName = request.Username,
            Email = request.Email,
            HashedPassword = hashedPassword
        };

        // Create teacher
        var teacher = new Teacher
        {
            Account = account
        };

        db.Teachers.Add(teacher);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new RegisterTeacherResponse
        {
            Id = teacher.Id,
            AccountId = account.Id,
            IsSuccess = true,
            Message = "✅ Successfully added a new teacher account!"
        });
    }
}
