using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Scrapers;

public class SeleniumScraper : IScraperStrategy
{
    public async Task<IEnumerable<PriceHistory>> ScrapeAsync(string keyword)
    {
        var results = new List<PriceHistory>();

        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");

        using var driver = new ChromeDriver(options);
        try
        {
            // Demo: navigate to a marketplace search
            driver.Navigate().GoToUrl($"https://www.tokopedia.com/search?st=product&q={Uri.EscapeDataString(keyword)}");
            await Task.Delay(3000); // wait for JS render

            var priceElements = driver.FindElements(By.CssSelector("[data-testid='lblProductPrice']"));
            foreach (var el in priceElements.Take(10))
            {
                var text = el.Text.Replace("Rp", "").Replace(".", "").Replace(",", "").Trim();
                if (decimal.TryParse(text, out var price))
                {
                    results.Add(new PriceHistory
                    {
                        ProductName = keyword,
                        Price = price,
                        Source = "Selenium/Tokopedia",
                        ScrapedAt = DateTime.UtcNow
                    });
                }
            }
        }
        catch
        {
            // Selenium may fail in CI/headless environments — return empty
        }
        finally
        {
            driver.Quit();
        }

        return results;
    }
}
