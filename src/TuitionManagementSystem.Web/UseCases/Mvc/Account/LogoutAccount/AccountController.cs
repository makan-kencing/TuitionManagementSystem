namespace TuitionManagementSystem.Web.UseCases.Mvc.Account.LogoutAccount;

using Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public sealed partial class AccountController(ILogger<AccountController> logger) : Controller
{
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

    [LoggerMessage(Level = LogLevel.Information, Message = "User {name} logged out at {time}.")]
    private partial void Log_UserLogout(string name, DateTime time);
}
