namespace SmartInventory.Application.DTOs.Stocks;

public class LowStockDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int MinStock { get; set; }
    public int Deficit => MinStock - CurrentQuantity;
}
