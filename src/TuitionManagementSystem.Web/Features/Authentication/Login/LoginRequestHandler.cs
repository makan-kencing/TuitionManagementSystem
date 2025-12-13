namespace TuitionManagementSystem.Web.Features.Authentication.Login;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Claims;
using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.User;

public sealed class LoginRequestHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<LoginRequest, Result<LoginResponse>>
{
    private static readonly PasswordHasher<object> ph = new();

    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var account = await db.Accounts
            .Where(a => a.Username == request.Username)
            .FirstOrDefaultAsync(cancellationToken);

        // do password checking even if account does not exist to prevent timing attack.
        if (!VerifyPassword(account, request.Password))
        {
            return Result<LoginResponse>.Unauthorized();
        }

        if (HasTwoFactor(account))
        {
            if (request.TwoFactorToken == null)
            {
                return Result<LoginResponse>.Success(new LoginResponse(LoginResponseStatus.TwoFactorRequired));
            }

            if (!VerifyTwoFactor(account, request.TwoFactorToken))
            {
                return Result<LoginResponse>.Unauthorized();
            }
        }

        // passed checks, configure authentications
        var session = this.CreateAccountSession(account);

        var principal = CreateAccountClaimsPrincipal(session);
        var authProperties = new AuthenticationProperties { IsPersistent = request.RememberMe };

        await httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        // save session to db
        await db.AccountSessions
            .AddAsync(session, cancellationToken)
            .ConfigureAwait(false);
        await db.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(new LoginResponse(LoginResponseStatus.Success));
    }

    private static bool VerifyPassword([NotNullWhen(true)] Account? account, string password)
    {
        //dummy verify password to simulate time taken if record exists
        const string dummyHash = "AQAAAAIAAYagAAAAEGyBGBzs6LQiyNF3QVuXc6Cz0yVIGikGDI6GdsXMFG6eDzkN0617JG54Tz/Zp/V35g==";

        var hashToCheck = account?.HashedPassword ?? dummyHash;

        var result = ph.VerifyHashedPassword(0, hashToCheck, password);

        return account != null && result == PasswordVerificationResult.Success;
    }

    private static bool HasTwoFactor(Account account)
    {
        // TODO: implement checking logic
        _ = "TODO";
        return false;
    }

    private static bool VerifyTwoFactor(Account account, string token)
    {
        // TODO: implement 2fa logic
        _ = "TODO";
        return true;
    }

    private AccountSession CreateAccountSession(Account account) =>
        new()
        {
            SessionId = Guid.NewGuid(),
            Account = account,
            LastIp = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress,
            LastLogin = DateTime.UtcNow
        };

    private static ClaimsPrincipal CreateAccountClaimsPrincipal(AccountSession session)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.Account.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Thumbprint, session.SessionId.ToString())
        };
        return new(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
        );
    }

}
