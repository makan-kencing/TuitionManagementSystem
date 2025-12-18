namespace TuitionManagementSystem.Web.Features.Family.CreateFamily;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.User;

public class CreateFamilyCommandHandler(ApplicationDbContext db) : IRequestHandler<CreateFamilyCommand, Result>
{
    public async Task<Result> Handle(CreateFamilyCommand request, CancellationToken cancellationToken)
    {
        var parent = await db.Parents
            .Include(p => p.Account)
            .Include(p => p.Family)
            .Where(p => p.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (parent == null)
        {
            return Result.Unauthorized();
        }

        if (parent.Family is not null)
        {
            return Result.Forbidden("Already has a family.");
        }

        var family = new Family
        {
            Name = $"{parent.Account.DisplayName}'s Family",
            Members = [new FamilyMember
            {
                User = parent
            }]
        };

        await db.Families.AddAsync(family, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
