namespace TuitionManagementSystem.Web.Features.Abstractions;

using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static ActionResult SeeOther(this Controller controller, string location)
    {
        controller.Response.Headers.Append("Location", location);
        return new StatusCodeResult(303);
    }
}
