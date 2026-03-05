using System.ComponentModel.DataAnnotations;

namespace SmartInventory.Application.DTOs.PurchaseOrders;

public class CreatePurchaseOrderDto
{
    [Required]
    public int SupplierId { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreatePODetailDto> Details { get; set; } = new();
}

public class CreatePODetailDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}
