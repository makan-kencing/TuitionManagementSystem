namespace TuitionManagementSystem.Web.Features.Family.GetFamily;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Extensions;

public class GetFamilyQueryHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetFamilyQuery, Result<GetFamilyResponse>>
{
    public async Task<Result<GetFamilyResponse>> Handle(GetFamilyQuery request, CancellationToken cancellationToken)
    {
        var accountId = httpContextAccessor.HttpContext?.User.GetUserId();

        if (accountId is null)
        {
            return Result.Unauthorized();
        }

        var user = await db.Users
            .Include(u => u.Family)
            .ThenInclude(f => f.Children)
            .Where(u => u.Id == accountId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        if (user.Family is null)
        {
            return Result.NotFound();
        }

        return Result.Success();
    }
}
