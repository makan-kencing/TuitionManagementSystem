namespace TuitionManagementSystem.Web.Features.Accounts.CheckEmail;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CheckEmailRequestHandler(
    ApplicationDbContext db) : IRequestHandler<CheckEmailRequest, Result<CheckEmailResponse>>
{
    public async Task<Result<CheckEmailResponse>> Handle(CheckEmailRequest request, CancellationToken cancellationToken)
    {
        var exists = await db.Account
            .AsNoTracking()
            .Where(a => a.Email == request.Email)
            .AnyAsync(cancellationToken);

        return Result<CheckEmailResponse>.Success(new CheckEmailResponse
        {
            Exists = exists
        });
    }
}
