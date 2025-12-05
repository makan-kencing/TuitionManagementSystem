namespace TuitionManagementSystem.Web.Features.Authentication.Logout;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Extensions;

public sealed class LogoutRequestHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor): IRequestHandler<LogoutRequest, Result>
{
    public async Task<Result> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        await db.AccountSessions
            .Where(s => s.SessionId == httpContextAccessor.HttpContext!.User.GetGuid())
            .ExecuteDeleteAsync(cancellationToken);

        await httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Result.Success();
    }
}
