using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.DTOs.Stocks;

public class StockMutationDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public MutationType Type { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
