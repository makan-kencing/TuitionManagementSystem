namespace TuitionManagementSystem.Web.Auth.Extensions;

using System.Globalization;
using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUsername(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name);

    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
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

        var value = user.FindFirstValue(ClaimTypes.Version);
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

    public static Guid? GetGuid(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.Thumbprint);
        return value == null ? null : Guid.Parse(value);
    }
}


