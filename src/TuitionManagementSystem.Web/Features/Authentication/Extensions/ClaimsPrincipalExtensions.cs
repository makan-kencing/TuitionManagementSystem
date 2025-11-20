namespace TuitionManagementSystem.Web.Features.Authentication.Extensions;

using System.Globalization;
using System.Security.Claims;
using Constants;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = GetClaimValue(user, InternalClaimTypes.UserId);
        try
        {
            return string.IsNullOrEmpty(value)
                ? null
                : int.Parse(value, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public static DateTime? GetLastChanged(this ClaimsPrincipal user)
    {
        var value = GetClaimValue(user, InternalClaimTypes.LastChanged);
        try
        {
            return value == null
                ? null
                : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static string? GetClaimValue(in ClaimsPrincipal user, string name)
        => user.Claims.FirstOrDefault(claim => claim.Type.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value;
}


