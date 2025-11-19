namespace TuitionManagementSystem.Web.Features.Authentication;

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

        var lastChanged = (from c in userPrincipal.Claims
            where c.Type == "LastChanged"
            select c.Value).FirstOrDefault();

        if (string.IsNullOrEmpty(lastChanged))
        {
            await InvalidateSession(context);
            return;
        }

        var dbLastChanged = await db.Account
            .AsNoTracking()
            .Where(a => a.Id == int.Parse((from c in userPrincipal.Claims
                where c.Type == "Id"
                select c.Value).First(), null))
            .Select(a => a.LastChanged)
            .FirstAsync();

        if (string.IsNullOrEmpty(lastChanged) || dbLastChanged != DateTime.Parse(lastChanged, null))
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
