using Microsoft.Extensions.Logging;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Infrastructure.Scrapers;

public class ScraperService : IScraperService
{
    private readonly IScraperStrategy _strategy;
    private readonly IPriceHistoryRepository _priceRepo;
    private readonly ILogger<ScraperService> _logger;

    public ScraperService(IScraperStrategy strategy, IPriceHistoryRepository priceRepo, ILogger<ScraperService> logger)
    {
        _strategy = strategy;
        _priceRepo = priceRepo;
        _logger = logger;
    }

    public async Task ScrapeAndSaveAsync(IEnumerable<string> keywords)
    {
        foreach (var keyword in keywords)
        {
            try
            {
                _logger.LogInformation("Scraping prices for keyword: {Keyword}", keyword);
                var results = await _strategy.ScrapeAsync(keyword);
                var list = results.ToList();
                if (list.Count > 0)
                {
                    await _priceRepo.AddRangeAsync(list);
                    _logger.LogInformation("Saved {Count} price records for '{Keyword}'", list.Count, keyword);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to scrape keyword: {Keyword}", keyword);
            }
        }
    }
}
