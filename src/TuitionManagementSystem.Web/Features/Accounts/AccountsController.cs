namespace TuitionManagementSystem.Web.Features.Accounts;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Extensions;
using Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public sealed partial class AccountsController(ILogger<AccountsController> logger) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody][Required] LoginViewModel login, Uri? returnUrl)
    {
        // Do fetching user here
        var user = true;

        if (user == null)
        {
            return this.Unauthorized();
        }

        var claims = new List<Claim>
        {
            new("Id", ""),
            new(ClaimTypes.Name, "name"),
            new(ClaimTypes.Role, "Member")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = login.RememberMe
        };

        await this.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        this.Log_UserLogin(login.Email, DateTime.UtcNow);

        return this.LocalRedirect(returnUrl?.LocalPath ?? "/");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        if (this.User.Identity == null)
        {
            return this.BadRequest();
        }

        this.Log_UserLogout(this.User.Identity.Name ?? string.Empty, DateTime.UtcNow);

        await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return this.LocalRedirect(this.Request.GetReferrer() ?? "/");
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "User {name} logged in at {time}.")]
    private partial void Log_UserLogin(string name, DateTime time);

    [LoggerMessage(Level = LogLevel.Information, Message = "User {name} logged out at {time}.")]
    private partial void Log_UserLogout(string name, DateTime time);
}
