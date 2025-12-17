namespace TuitionManagementSystem.Web.Features.Family.DeleteFamily;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.User;

public class DeleteFamilyCommandHandler(ApplicationDbContext db) : IRequestHandler<DeleteFamilyCommand, Result>
{
    public async Task<Result> Handle(DeleteFamilyCommand request, CancellationToken cancellationToken)
    {
        var family = await db.FamilyMembers
            .AsNoTracking()
            .Where(fm => fm.User.Id == request.UserId)
            .Where(fm => fm.User is Parent)
            .Select(fm => fm.Family)
            .FirstOrDefaultAsync(cancellationToken);

        if (family is null)
        {
            return Result.Forbidden();
        }

        if (family.Parents.Count() > 1)
        {
            return Result.Forbidden();
        }

        await db.FamilyMembers
            .Where(fm => fm.Family.Id == family.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await db.Families
            .Where(f => f.Id == family.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
}
