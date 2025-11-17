namespace TuitionManagementSystem.Web.Modules;

using Domain;
using Domain.Entities.Account;
using Infrastructure.DataAccess.Repositories;

public static class UseRepositories
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IRepository<Account>, Repository<Account>>();

        return services;
    }
}
