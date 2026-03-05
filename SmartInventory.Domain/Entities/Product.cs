namespace SmartInventory.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinStock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Stock? Stock { get; set; }
    public ICollection<StockMutation> StockMutations { get; set; } = new List<StockMutation>();
    public ICollection<PODetail> PODetails { get; set; } = new List<PODetail>();
}
