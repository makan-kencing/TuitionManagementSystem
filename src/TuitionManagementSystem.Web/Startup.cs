namespace TuitionManagementSystem.Web;

using System.Configuration;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Options;
using Services.Auth;
using Services.Email;
using Services.View;
using Features.Authentication.Login;
using Features.Family;
using Features.Family.CheckInvite;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Models.Notification;
using Services.File;
using Services.Payment;

public class Startup(IConfiguration configuration)
{
    private readonly LuckyPennySoftwareOptions luckyPennySoftwareOptions = configuration
        .GetSection("LuckyPennySoftware")
        .Get<LuckyPennySoftwareOptions>() ?? throw new ConfigurationErrorsException();

    private readonly string connectionString = configuration
        .GetConnectionString("DefaultConnection") ?? throw new ConfigurationErrorsException();

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
            .AddMediatR(cfg =>
            {
                cfg.LicenseKey = this.luckyPennySoftwareOptions.LicenseKey;
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            })
            .AddAutoMapper(cfg =>
            {
                cfg.CreateMap<LoginViewModel, LoginRequest>();
                cfg.CreateMap<CheckInviteResponse, FamilyInvite>();
            });

        services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(this.connectionString)
                    .UseSeeding((context, _) =>
                    {
                        var seed = new SeedData(context);
                        seed.InitializeAsync().GetAwaiter().GetResult();
                    })
                    .UseAsyncSeeding(async (context, _, cancellationToken) =>
                    {
                        var seed = new SeedData(context);
                        await seed.InitializeAsync(cancellationToken);
                    }))
            .AddHttpContextAccessor()
            .AddProblemDetails()
            .AddSwaggerGen()
            .AddRequestTimeouts()
            .AddAntiforgery()
            .AddResponseCompression()
            .AddResponseCaching()
            .AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // session timeout
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services
            .AddSignalR();

        services
            .AddAuthorization(options =>
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build())
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.EventsType = typeof(UserCookieAuthenticationEvents);
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });

        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddSingleton<IPaymentService, StripePaymentService>();
        services.AddSingleton<IFileService, PhysicalFileService>();
        services.AddSingleton<IAuthorizationHandler, FamilyAuthorizationHandler>();

        services.AddScoped<UserCookieAuthenticationEvents>();
        services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

        services.AddTransient<IClaimsTransformation, AccountClaimsTransformer>();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IFileService fileService)
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
                FileProvider = fileService.FileProvider, RequestPath = fileService.MappedPath
            })
            .UseSession()
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
