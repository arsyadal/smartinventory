using SmartInventory.Domain.Enums;
using SmartInventory.Domain.Exceptions;

namespace SmartInventory.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }

    public Supplier? Supplier { get; set; }
    public ICollection<PODetail> Details { get; set; } = new List<PODetail>();

    public void Confirm()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Only Draft orders can be confirmed.", "INVALID_STATUS_TRANSITION");
        Status = PurchaseOrderStatus.Confirmed;
    }

    public void MarkReceived()
    {
        if (Status != PurchaseOrderStatus.Confirmed)
            throw new DomainException("Only Confirmed orders can be marked as received.", "INVALID_STATUS_TRANSITION");
        Status = PurchaseOrderStatus.Received;
        ReceivedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Received)
            throw new DomainException("Received orders cannot be cancelled.", "INVALID_STATUS_TRANSITION");
        if (Status == PurchaseOrderStatus.Cancelled)
            throw new DomainException("Order is already cancelled.", "INVALID_STATUS_TRANSITION");
        Status = PurchaseOrderStatus.Cancelled;
    }
}
