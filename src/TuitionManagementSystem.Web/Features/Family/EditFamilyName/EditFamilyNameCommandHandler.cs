namespace TuitionManagementSystem.Web.Features.Family.EditFamilyName;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class EditFamilyNameCommandHandler(ApplicationDbContext db) : IRequestHandler<EditFamilyNameCommand, Result>
{
    public async Task<Result> Handle(EditFamilyNameCommand request, CancellationToken cancellationToken)
    {
        var familyId = await db.FamilyMembers
            .Where(fm => fm.User.Id == request.UserId)
            .Select(fm => fm.FamilyId)
            .FirstOrDefaultAsync(cancellationToken);

        var updated = await db.Families
            .Where(f => f.Id == familyId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(f => f.Name, request.Name), cancellationToken);

        return updated > 0 ? Result.Success() : Result.NotFound();
    }
}
