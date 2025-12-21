namespace TuitionManagementSystem.Web.Features.Family.GetFamily;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetFamilyQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetFamilyQuery, Result<GetFamilyResponse>>
{
    public async Task<Result<GetFamilyResponse>> Handle(GetFamilyQuery request, CancellationToken cancellationToken)
    {
        var response = await db.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => u.Family!.Family)
            .Select(f => new GetFamilyResponse
            {
                Name = f.Name,
                Members = f.Members.Select(fm => new FamilyMember
                {
                    User = new FamilyUser
                    {
                        Id = fm.User.Id,
                        AccountUsername = fm.User.Account.Username,
                        AccountDisplayName = fm.User.Account.DisplayName,
                        AccountProfileImageUri = fm.User.Account.ProfileImage != null
                            ? fm.User.Account.ProfileImage.Uri
                            : null,
                        Type = fm.User.GetType().Name
                    },
                    JoinedOn = fm.JoinedOn
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return Result.Unauthorized();
        }

        if (response.Members.Count == 0)
        {
            return Result.NotFound();
        }

        return Result.Success(response);
    }
}
