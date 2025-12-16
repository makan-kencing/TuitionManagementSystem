namespace TuitionManagementSystem.Web.Services.Background
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MediatR;
    using TuitionManagementSystem.Web.Services;

    public class MonthlyInvoiceService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MonthlyInvoiceService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public MonthlyInvoiceService(
            IServiceProvider serviceProvider,
            ILogger<MonthlyInvoiceService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monthly Invoice Background Service started");

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Checking for monthly invoice generation");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                        await invoiceService.GenerateMonthlyInvoicesAsync(mediator, stoppingToken);
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error in monthly invoice service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Monthly Invoice Background Service stopped");
        }
    }
}
