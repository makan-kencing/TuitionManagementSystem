namespace TuitionManagementSystem.Web;

using System.Reflection;
using Features.Abstractions;
using Features.Authentication;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

public class Startup(IConfiguration configuration)
{
    private static readonly Assembly AssemblyToScan = typeof(IFeatureMarker).Assembly;

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString))
            .AddHttpContextAccessor()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyToScan))
            .AddControllersWithViews();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.EventsType = typeof(UserCookieAuthenticationEvents);
            });

        services.AddScoped<UserCookieAuthenticationEvents>();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app
                .UseExceptionHandler("/Home/Error")
                .UseHsts();  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        app
            .UseSwagger()
            .UseSwaggerUI()
            .UseHttpsRedirection()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapStaticAssets();
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();
            });
    }
}
