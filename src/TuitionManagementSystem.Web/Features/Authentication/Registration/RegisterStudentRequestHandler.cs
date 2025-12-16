namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.User;

public sealed class RegisterStudentRequestHandler(ApplicationDbContext db)
    : IRequestHandler<RegisterStudentRequest, Result<RegisterStudentResponse>>
{
    public async Task<Result<RegisterStudentResponse>> Handle(
        RegisterStudentRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (await db.Accounts.AnyAsync(a => a.Username == request.Username, cancellationToken))
            errors.Add("⚠ Username already exists!");

        if (await db.Accounts.AnyAsync(a => a.Email == request.Email, cancellationToken))
            errors.Add("⚠ Email already exists!");

        if (errors.Any())
        {
            return Result.Success(new RegisterStudentResponse
            {
                IsSuccess = false,
                Message = string.Join("<br/>", errors) // join all errors with line breaks
            });
        }

        var hasher = new PasswordHasher<Account>();
        var hashedPassword = hasher.HashPassword(null, request.Password);

        var account = new Account
        {
            Username = request.Username,
            DisplayName = request.Username,
            Email = request.Email,
            HashedPassword = hashedPassword
        };

        var student = new Student { Account = account };

        db.Students.Add(student);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new RegisterStudentResponse
        {
            Id = student.Id,
            AccountId = account.Id,
            Message = "✅ Successfully added a new student account!",
            IsSuccess = true
        });
    }
}

