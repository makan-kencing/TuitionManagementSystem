namespace TuitionManagementSystem.Web.Auth.Logout;

using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public sealed class LogoutRequestHandler(
    IHttpContextAccessor httpContextAccessor): IRequestHandler<LogoutRequest, Result>
{
    public async Task<Result> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        await httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Result.Success();
    }
}
