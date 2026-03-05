namespace SmartInventory.Application.DTOs.Prices;

public class PriceHistoryDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime ScrapedAt { get; set; }
}

public class PriceTrendDto
{
    public string ProductName { get; set; } = string.Empty;
    public decimal AvgPrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public DateTime Date { get; set; }
}
