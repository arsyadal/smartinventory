using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.PurchaseOrders;

namespace SmartInventory.Application.Interfaces.Services;

public interface IPurchaseOrderService
{
    Task<ApiResponse<IEnumerable<PurchaseOrderDto>>> GetAllAsync();
    Task<ApiResponse<PurchaseOrderDto>> GetByIdAsync(int id);
    Task<ApiResponse<PurchaseOrderDto>> CreateAsync(CreatePurchaseOrderDto dto);
    Task<ApiResponse<PurchaseOrderDto>> ConfirmAsync(int id);
    Task<ApiResponse<PurchaseOrderDto>> ReceiveOrderAsync(int id);
    Task<ApiResponse<PurchaseOrderDto>> CancelAsync(int id);
}
