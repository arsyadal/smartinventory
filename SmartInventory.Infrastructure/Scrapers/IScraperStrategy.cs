using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Scrapers;

public interface IScraperStrategy
{
    Task<IEnumerable<PriceHistory>> ScrapeAsync(string keyword);
}
