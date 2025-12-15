namespace TuitionManagementSystem.Web.Features.Family.RemoveMember;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Services.Auth.Extensions;

public class RemoveMemberCommandHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var accountId = httpContextAccessor.HttpContext?.User.GetUserId();

        if (accountId is null)
        {
            return Result.Unauthorized();
        }

        var parent = await db.Parents
            .Where(u => u.Account.Id == accountId)
            .FirstOrDefaultAsync(cancellationToken);

        switch (parent)
        {
            case null:
                return Result.Unauthorized();
            case { Family: null }:
                return Result.Invalid();
        }

        var toRemove = await db.Users
            .Where(u => u.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        switch (toRemove)
        {
            case null:
                return Result.NotFound();
            case { Family: null }:
                return Result.Invalid();
            case { Family: not null } when toRemove.Family.Id != parent.Family.Id:
                return Result.Forbidden();
        }

        toRemove.Family = null;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
