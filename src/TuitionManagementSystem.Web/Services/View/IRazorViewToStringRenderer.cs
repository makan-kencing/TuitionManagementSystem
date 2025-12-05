namespace TuitionManagementSystem.Web.Services.View;

using JetBrains.Annotations;

public interface IRazorViewToStringRenderer
{
    public Task<string> RenderViewToStringAsync<TModel>([AspMvcView] string viewName, [AspMvcModelType] TModel model);
}
