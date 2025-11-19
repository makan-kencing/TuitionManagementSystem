namespace TuitionManagementSystem.Web.Extensions;

public static class RequestExtension
{
    public static string? GetReferrer(this HttpRequest request) => request.GetTypedHeaders().Referer?.ToString();
}
