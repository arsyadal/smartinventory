using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Scrapers;

public class MockMarketplaceScraper : IScraperStrategy
{
    private static readonly string[] Sources = { "Tokopedia", "Shopee", "Lazada", "Blibli" };
    private static readonly Random Rng = new();

    public Task<IEnumerable<PriceHistory>> ScrapeAsync(string keyword)
    {
        // Generate 5–10 realistic mock price records per keyword
        var count = Rng.Next(5, 11);
        var results = Enumerable.Range(0, count).Select(_ => new PriceHistory
        {
            ProductName = keyword,
            Price = Math.Round((decimal)(Rng.NextDouble() * 900_000 + 100_000), 2),
            Source = Sources[Rng.Next(Sources.Length)],
            ScrapedAt = DateTime.UtcNow.AddMinutes(-Rng.Next(0, 60))
        });

        return Task.FromResult(results);
    }
}
