namespace SmartInventory.Domain.Entities;

public class PODetail
{
    public int Id { get; set; }
    public int POId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public PurchaseOrder? PurchaseOrder { get; set; }
    public Product? Product { get; set; }
}
