namespace TuitionManagementSystem.Web;

public static class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, builder) => builder.AddCommandLine(args))
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
}
