namespace TuitionManagementSystem.Web.Features.Authentication.Login;

using System.Globalization;
using System.Security.Claims;
using Ardalis.Result;
using Constants;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

public sealed class LoginRequestHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor): IRequestHandler<LoginRequest, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var account = await db.Accounts
            .AsNoTracking()
            .Where(a => a.Username == request.Username)
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            return Result<LoginResponse>.Unauthorized();
        }

        // TODO: verify password with hash from db
        // maybe 2fa

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, account.AccessRole.ToString()),
            new(InternalClaimTypes.UserId, account.Id.ToString(CultureInfo.InvariantCulture)),
            new(InternalClaimTypes.LastChanged, account.LastChanged.ToString("o", CultureInfo.InvariantCulture))
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = request.RememberMe
        };

        await httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);

        return Result<LoginResponse>.Success(new LoginResponse(LoginResponseStatus.Success));
    }
}
