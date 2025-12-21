namespace TuitionManagementSystem.Web.Features.Authentication.TwoFactor;

using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Models.User;

[AllowAnonymous]
[Route("auth/twofactor")]
public class TwoFactorController(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : Controller
{
    [HttpGet("verify")]
    public async Task<IActionResult> Verify(string token)
    {
        var account = await db.Accounts
            .FirstOrDefaultAsync(a =>
                a.TwoFactorToken == token &&
                a.TwoFactorTokenExpiry > DateTime.UtcNow);

        if (account == null)
            return Unauthorized("Invalid or expired token");

        // Clear token
        account.TwoFactorToken = null;
        account.TwoFactorTokenExpiry = null;

        var session = new AccountSession
        {
            SessionId = Guid.NewGuid(),
            Account = account,
            LastIp = HttpContext.Connection.RemoteIpAddress,
            LastLogin = DateTime.UtcNow
        };

        db.AccountSessions.Add(session);
        await db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Thumbprint, session.SessionId.ToString())
        };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", "Home");
    }
}
