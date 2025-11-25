namespace TuitionManagementSystem.Web;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Features.Abstractions;
using Features.Authentication;
using Features.Authentication.Login;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

public class Startup(IConfiguration configuration)
{
    private static readonly Assembly AssemblyToScan = typeof(IFeatureMarker).Assembly;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .Configure<KestrelServerOptions>(options => options.AddServerHeader = true)
            .Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<JsonOptions>(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.WriteIndented = false;
                jsonOptions.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                jsonOptions.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                jsonOptions.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyToScan))
            .AddAutoMapper(cfg =>
            {
                cfg.CreateMap<LoginViewModel, LoginRequest>();
            });

        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            $"Host={configuration["DB_HOST"]};" +
            $"Port={configuration["DB_PORT"]};" +
            $"Username={configuration["DB_USER"]};" +
            $"Password={configuration["DB_PASSWORD"]};" +
            $"Database={configuration["DB_NAME"]}";

        services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString))
            .AddHttpContextAccessor()
            .AddResponseCompression()
            .AddEndpointsApiExplorer()
            .AddProblemDetails()
            .AddSwaggerGen()
            .AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

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
                .UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        app
            .UseSwagger()
            .UseSwaggerUI()
            .UseHttpsRedirection()
            .UseResponseCompression()
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
