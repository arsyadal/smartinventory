using SmartInventory.Domain.Enums;

namespace SmartInventory.Domain.Entities;

public class StockMutation
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public MutationType Type { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public Product? Product { get; set; }
}
