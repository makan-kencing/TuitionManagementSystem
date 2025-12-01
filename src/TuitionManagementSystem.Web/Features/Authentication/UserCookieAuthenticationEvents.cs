namespace TuitionManagementSystem.Web.Features.Authentication;

using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

public class UserCookieAuthenticationEvents(
    ApplicationDbContext db) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal;

        if (userPrincipal == null)
        {
            await InvalidateSession(context);
            return;
        }

        var id = userPrincipal.GetUserId();
        var lastChanged = userPrincipal.GetLastChanged();

        if (id == null || lastChanged == null)
        {
            await InvalidateSession(context);
            return;
        }

        var dbLastChanged = await db.Accounts
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => a.LastChanged)
            .FirstAsync();

        if (dbLastChanged != lastChanged)
        {
            await InvalidateSession(context);
        }
    }

    private static async Task InvalidateSession(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
