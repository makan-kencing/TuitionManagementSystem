namespace TuitionManagementSystem.Web.Services.Auth.Extensions;

using System.Globalization;
using System.Security.Claims;
using Constants;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUsername(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name);

    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(InternalClaimTypes.UserId);
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

    public static int? GetAccountId(this ClaimsPrincipal user)
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

    public static string? GetUserType(this ClaimsPrincipal user) =>
        user.FindFirstValue(InternalClaimTypes.UserType);

    public static string? GetProfileImageUri(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Uri);

    public static string? GetDisplayName(this ClaimsPrincipal user) =>
        user.FindFirstValue(InternalClaimTypes.DisplayName);

    public static string? GetUserEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email);
}


