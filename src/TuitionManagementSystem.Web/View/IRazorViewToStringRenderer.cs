namespace TuitionManagementSystem.Web.View;

public interface IRazorViewToStringRenderer
{
    public Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
}
