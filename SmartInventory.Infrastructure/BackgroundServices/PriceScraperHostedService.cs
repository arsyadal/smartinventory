using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Infrastructure.BackgroundServices;

public class PriceScraperHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceScraperHostedService> _logger;
    private readonly TimeSpan _interval;

    // Default keywords to scrape; in production these could be stored in DB
    private static readonly string[] DefaultKeywords = { "laptop gaming", "smartphone", "headphone wireless" };

    public PriceScraperHostedService(IServiceScopeFactory scopeFactory, ILogger<PriceScraperHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _interval = TimeSpan.FromHours(1);
    }

    public PriceScraperHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<PriceScraperHostedService> logger,
        double intervalHours)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _interval = TimeSpan.FromHours(intervalHours);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Price scraper background service started. Interval: {Interval}", _interval);

        // Run immediately on startup, then at interval
        await RunScraperAsync();

        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunScraperAsync();
        }
    }

    private async Task RunScraperAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var scraperService = scope.ServiceProvider.GetRequiredService<IScraperService>();
            await scraperService.ScrapeAndSaveAsync(DefaultKeywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in price scraper background service.");
        }
    }
}
