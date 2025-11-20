namespace TuitionManagementSystem.Web.Extensions;

public static class RequestExtension
{
    public static Uri? GetReferrer(this HttpRequest request) => request.GetTypedHeaders().Referer;
}
