namespace TuitionManagementSystem.Web.Features.Authentication;

using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using AutoMapper;
using Login;
using Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

[Route("/")]
public sealed class AuthenticationController(
    IMediator mediator,
    IMapper mapper) : Controller
{
    [HttpGet]
    public IActionResult Login() => this.View();

    [HttpPost]
    public async Task<IActionResult> Login([FromBody][Required] LoginViewModel login, Uri? returnUrl)
    {
        var result = await mediator.Send(mapper.Map<LoginRequest>(login));

        if (result.IsOk())
        {
            return this.LocalRedirect(returnUrl?.LocalPath ?? "/");
        }

        if (result.IsUnauthorized() && result.Value.ErrorStatus == LoginResponseStatus.TwoFactorRequired)
        {
            return this.View("TwoFactor", login);
        }

        // Error
        return this.View(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> TwoFactor() =>
        throw new NotImplementedException();


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await mediator.Send(new LogoutRequest());
        return this.LocalRedirect(this.Request.GetReferrer() ?? "/");
    }
}
