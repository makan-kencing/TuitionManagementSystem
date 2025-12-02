namespace TuitionManagementSystem.Web;

using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Features.Abstractions;
using Features.Authentication;
using Features.Authentication.Login;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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
            })
            .Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
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
            .AddProblemDetails()
            .AddSwaggerGen()
            .AddEndpointsApiExplorer()
            .AddRequestTimeouts()
            .AddCors()
            .AddAntiforgery()
            .AddResponseCompression()
            .AddResponseCaching()
            .AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        services
            .AddSignalR();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.EventsType = typeof(UserCookieAuthenticationEvents);
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });

        services.AddScoped<UserCookieAuthenticationEvents>();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0#order
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage()
                .UseForwardedHeaders();
        }
        else
        {
            app
                .UseExceptionHandler("/Home/Error")
                .UseForwardedHeaders()
                .UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        app
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseSwagger()
            .UseSwaggerUI()
            .UseRouting()
            .UseRequestTimeouts()
            .UseCors()
            .UseAuthentication()
            .UseAuthorization()
            .UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "assets")),
                RequestPath = "/assets",
                OnPrepareResponse = ctx =>
                {
                    if (!ctx.Context.User.Identity!.IsAuthenticated)
                    {
                        ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                }
            })
            // .UseSession()
            .UseAntiforgery()
            .UseResponseCompression()
            .UseResponseCaching()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapStaticAssets();
                endpoints.MapDefaultControllerRoute();
            });
    }
}
