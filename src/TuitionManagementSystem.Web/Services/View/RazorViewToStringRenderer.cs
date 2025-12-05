namespace TuitionManagementSystem.Web.Services.View;

using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

// https://www.endpointdev.com/blog/2024/04/using-razor-templates-to-render-emails-dotnet/
public class RazorViewToStringRenderer(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider) : IRazorViewToStringRenderer
{
    public async Task<string> RenderViewToStringAsync<TModel>([AspMvcView] string viewName, [AspMvcModelType] TModel model)
    {
        var actionContext = this.GetActionContext();

        // Find the view using the view engine
        var viewResult = viewEngine.FindView(actionContext, viewName, false);
        if (!viewResult.Success)
        {
            throw new FileNotFoundException($"View '{viewName}' not found.");
        }

        // Get the view context
        using var writer = new StringWriter();

        var view = viewResult.View;
        var viewDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };
        var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);
        var viewContext = new ViewContext(
            actionContext,
            view,
            viewDictionary,
            tempData,
            writer,
            new HtmlHelperOptions()
        );

        // Render the view to a string
        await view.RenderAsync(viewContext);
        return writer.ToString();
    }

    private ActionContext GetActionContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = serviceProvider;
        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }
}
