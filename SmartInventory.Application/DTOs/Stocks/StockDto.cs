namespace SmartInventory.Application.DTOs.Stocks;

public class StockDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsLowStock => Quantity < MinStock;
}
