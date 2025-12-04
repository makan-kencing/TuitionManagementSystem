namespace TuitionManagementSystem.Web.Features.Authentication;

using System.Globalization;
using System.Security.Claims;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

public class AccountClaimsTransformer(ApplicationDbContext db) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var account = await db.Accounts
            .Where(a => a.Id == principal.GetUserId())
            .Include(a => a.ProfileImage)
            .FirstOrDefaultAsync();

        // No new claims to make
        if (account == null || account.LastChanged == principal.GetLastChanged())
        {
            return principal;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, account.Username),
            new(ClaimTypes.Role, account.AccessRole.ToString()),
            new(ClaimTypes.Version, account.LastChanged.ToString("o", CultureInfo.InvariantCulture))
        };

        if (account.ProfileImage != null)
        {
            claims.Add(new(ClaimTypes.Uri, account.ProfileImage.Uri.ToString()));
        }

        var identity = principal.Identity as ClaimsIdentity;
        identity!.AddClaims(claims);

        return principal;
    }
}
