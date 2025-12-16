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
        if (await db.Accounts.AnyAsync(a => a.Username == request.Username, cancellationToken))
            return Result.Conflict("Username already exists");

        if (await db.Accounts.AnyAsync(a => a.Email == request.Email, cancellationToken))
            return Result.Conflict("Email already exists");

        var hasher = new PasswordHasher<Account>();
        var hashedPassword = hasher.HashPassword(null, request.Password);

        var account = new Account
        {
            Username = request.Username,
            Email = request.Email,
            HashedPassword = hashedPassword
        };

        var student = new Student
        {
            Account = account
        };

        db.Students.Add(student);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new RegisterStudentResponse
        {
            Id = student.Id,
            AccountId = account.Id
        });
    }
}

