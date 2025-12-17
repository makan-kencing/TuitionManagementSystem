namespace TuitionManagementSystem.Web.Services.Auth.Extensions;

using System.Globalization;
using System.Security.Claims;
using Constants;

public static class ClaimsPrincipalExtensions
{
    private static T? FindFirstValue<T>(this ClaimsPrincipal principal, string claimType, Func<string, T> converter)
        where T : struct
    {
        var value = principal.FindFirstValue(claimType);
        return value is null ? default : converter.Invoke(value);
    }

    public static int GetAccountId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier,
            v => int.Parse(v, CultureInfo.InvariantCulture))
        ?? throw new KeyNotFoundException();

    public static string GetUsername(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name) ?? throw new KeyNotFoundException();

    public static string? GetDisplayName(this ClaimsPrincipal user) =>
        user.FindFirstValue(InternalClaimTypes.DisplayName);

    public static DateTime? GetLastChanged(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Version,
            v => DateTime.Parse(v, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal))
        ?? throw new KeyNotFoundException();

    public static string? GetProfileImageUri(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Uri);

    public static Guid GetGuid(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Thumbprint, Guid.Parse) ?? throw new KeyNotFoundException();

    public static int GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(InternalClaimTypes.UserId,
            v => int.Parse(v, CultureInfo.InvariantCulture))
        ?? throw new KeyNotFoundException();

    public static string GetUserType(this ClaimsPrincipal user) =>
        user.FindFirstValue(InternalClaimTypes.UserType) ?? throw new KeyNotFoundException();
}
