namespace TuitionManagementSystem.Web.Services.Auth;

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
        var principal = context.Principal;
        if (principal == null)
        {
            return;
        }

        var sessionId = principal.GetGuid();
        if (!await db.AccountSessions
                .Where(s => s.SessionId == sessionId)
                .AnyAsync())
        {
            context.RejectPrincipal();

            await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        await db.AccountSessions
            .Where(a => a.SessionId == sessionId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.LastIp, context.HttpContext.Connection.RemoteIpAddress)
                .SetProperty(a => a.LastLogin, DateTime.UtcNow));
    }
}
