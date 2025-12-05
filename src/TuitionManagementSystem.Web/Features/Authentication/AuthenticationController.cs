namespace TuitionManagementSystem.Web.Auth;

using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using AutoMapper;
using Login;
using Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/[action]")]
public sealed class AuthenticationController(
    IMediator mediator,
    IMapper mapper) : Controller
{
    [HttpGet]
    public IActionResult Login() => this.View();

    [HttpPost]
    public async Task<IActionResult> Login([FromForm][Required] LoginViewModel login, [FromQuery] Uri? returnUrl)
    {
        var result = await mediator.Send(mapper.Map<LoginRequest>(login));

        if (!result.IsOk())
        {
            // Error
            this.ModelState.AddModelError("Invalid", "Username or password is incorrect.");
            return this.View(login);
        }

        if (result.Value.Status == LoginResponseStatus.TwoFactorRequired)
        {
            return this.View("TwoFactor", login);
        }
        return this.LocalRedirect(returnUrl?.OriginalString ?? "/");
    }

    [HttpPost]
    public async Task<IActionResult> TwoFactor() =>
        throw new NotImplementedException();

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout([FromHeader] Uri? referer)
    {
        await mediator.Send(new LogoutRequest());
        return this.LocalRedirect(referer?.LocalPath ?? "/");
    }
}
