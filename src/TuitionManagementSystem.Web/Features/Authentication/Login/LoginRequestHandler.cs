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
        var user = await db.User
            .AsNoTracking()
            .Where(a => a.Username == request.Username)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result<LoginResponse>.Unauthorized();
        }

        // TODO: verify password with hash from db
        // maybe 2fa

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, nameof(user.AccessRole)),
            new(InternalClaimTypes.UserId, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(InternalClaimTypes.LastChanged, user.LastChanged.ToString("o", CultureInfo.InvariantCulture))
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
