namespace TuitionManagementSystem.Web.Services.Auth;

using System.Globalization;
using System.Security.Claims;
using Constants;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

public class AccountClaimsTransformer(ApplicationDbContext db) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Already has claims, No need to add claims
        if (principal.HasClaim(c => c.Type == ClaimTypes.Name))
        {
            return principal;
        }

        var account = await db.Accounts
            .Where(a => a.Id == principal.GetAccountId())
            .Include(a => a.User)
            .Include(a => a.ProfileImage)
            .FirstOrDefaultAsync();

        if (account == null)
        {
            return principal;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, account.Username),
            new(ClaimTypes.Role, account.AccessRole.ToString())
        };

        if (account.User is not null)
        {
            claims.AddRange([
                new Claim(InternalClaimTypes.UserId, account.User.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(InternalClaimTypes.UserType, account.User.GetType().Name)
            ]);
        }

        if (account.ProfileImage is not null)
        {
            claims.Add(new Claim(ClaimTypes.Uri, account.ProfileImage.Uri.ToString()));
        }

        var identity = principal.Identity as ClaimsIdentity;
        identity!.AddClaims(claims);

        return principal;
    }
}
