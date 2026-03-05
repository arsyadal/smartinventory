using HtmlAgilityPack;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Scrapers;

public class HtmlAgilityPackScraper : IScraperStrategy
{
    public async Task<IEnumerable<PriceHistory>> ScrapeAsync(string keyword)
    {
        // Static HTML scraping demonstration
        // In production, target a static marketplace search result page
        var web = new HtmlWeb();
        var results = new List<PriceHistory>();

        try
        {
            // Demo: scrape a public static page (placeholder — adapt URL per target)
            var doc = await Task.Run(() => web.Load($"https://www.example.com/search?q={Uri.EscapeDataString(keyword)}"));

            var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");
            if (priceNodes != null)
            {
                foreach (var node in priceNodes.Take(10))
                {
                    var text = node.InnerText.Trim().Replace("Rp", "").Replace(".", "").Replace(",", "").Trim();
                    if (decimal.TryParse(text, out var price))
                    {
                        results.Add(new PriceHistory
                        {
                            ProductName = keyword,
                            Price = price,
                            Source = "HtmlAgilityPack",
                            ScrapedAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }
        catch
        {
            // Fallback to empty if scraping fails
        }

        return results;
    }
}
