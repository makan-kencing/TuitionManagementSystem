namespace TuitionManagementSystem.Web.Features.Family.LeaveFamily;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class LeaveFamilyCommandHandler(ApplicationDbContext db) : IRequestHandler<LeaveFamilyCommand, Result>
{
    public async Task<Result> Handle(LeaveFamilyCommand request, CancellationToken cancellationToken)
    {
        await db.FamilyMembers
            .Where(fm => fm.User.Id == request.UserId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
}
