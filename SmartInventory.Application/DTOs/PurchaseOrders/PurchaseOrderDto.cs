using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.DTOs.PurchaseOrders;

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public List<PODetailDto> Details { get; set; } = new();
}
