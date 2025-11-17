namespace TuitionManagementSystem.Web.Modules;

using Application.UseCases.CheckEmail;

public static class UseCasesExtension
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICheckEmailUseCase, CheckEmailUseCase>();

        return services;
    }
}
