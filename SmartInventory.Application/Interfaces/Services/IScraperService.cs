namespace SmartInventory.Application.Interfaces.Services;

public interface IScraperService
{
    Task ScrapeAndSaveAsync(IEnumerable<string> keywords);
}
