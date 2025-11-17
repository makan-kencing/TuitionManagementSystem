namespace TuitionManagementSystem.Web.UseCases.Mvc.Account.LoginAccount;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public sealed partial class AccountController(ILogger<AccountController> logger) : Controller
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
            new(ClaimTypes.Name, "name"),
            new(ClaimTypes.Email, "email"),
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

    [LoggerMessage(Level = LogLevel.Information, Message = "User {name} logged in at {time}.")]
    private partial void Log_UserLogin(string name, DateTime time);
}
