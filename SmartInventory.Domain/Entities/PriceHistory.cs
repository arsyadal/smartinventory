namespace SmartInventory.Domain.Entities;

public class PriceHistory
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime ScrapedAt { get; set; }
}
