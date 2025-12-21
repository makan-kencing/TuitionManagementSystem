namespace TuitionManagementSystem.Web.Services.Background
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MediatR;
    using TuitionManagementSystem.Web.Services;

    public class OverdueInvoiceService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OverdueInvoiceService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); //check one every 1 hour

        public OverdueInvoiceService(
            IServiceProvider serviceProvider,
            ILogger<OverdueInvoiceService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Overdue Invoice Background Service started");

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Starting scheduled overdue invoice check");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                        await invoiceService.CheckAndCreateOverdueInvoicesAsync(mediator, stoppingToken);
                    }

                    _logger.LogInformation("Scheduled overdue invoice check completed");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error in scheduled overdue invoice check");
                }

                _logger.LogDebug("Waiting {Hours} hours until next check", _checkInterval.TotalHours);
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Overdue Invoice Background Service stopped");
        }
    }
}
