namespace TuitionManagementSystem.Web.Features.Authentication.Extensions;

using System.Security.Claims;
using Constants;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = GetClaimValue(user, InternalClaimTypes.UserId);
        return string.IsNullOrEmpty(value)
            ? null
            : int.Parse(value, null);
    }

    public static DateTime? GetLastChanged(this ClaimsPrincipal user)
    {
        var value = GetClaimValue(user, InternalClaimTypes.LastChanged);
        return value == null
            ? null
            : DateTime.Parse(value, null);
    }

    private static string? GetClaimValue(in ClaimsPrincipal user, string name)
        => user.Claims.FirstOrDefault(claim => claim.Type.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value;
}


