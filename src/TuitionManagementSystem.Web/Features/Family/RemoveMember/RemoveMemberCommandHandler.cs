namespace TuitionManagementSystem.Web.Features.Family.RemoveMember;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Extensions;

public class RemoveMemberCommandHandler(ApplicationDbContext db) : IRequestHandler<RemoveMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var parent = await db.FamilyMembers
            .AsNoTracking()
            .Where(fm => fm.User.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (parent is null)
        {
            return Result.Unauthorized();
        }

        var canRemove = await db.FamilyMembers
            .Where(fm => fm.Family.Id == parent.FamilyId)
            .Where(fm => fm.User.Id == request.RemoveUserId)
            .AnyAsync(cancellationToken);

        if (!canRemove)
        {
            return Result.Forbidden();
        }

        await db.FamilyMembers
            .Where(fm => fm.Family.Id == parent.FamilyId)
            .Where(fm => fm.User.Id == request.RemoveUserId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
}
