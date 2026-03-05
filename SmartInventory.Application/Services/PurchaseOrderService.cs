using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.PurchaseOrders;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IPurchaseOrderRepository _poRepo;
    private readonly IStockRepository _stockRepo;
    private readonly IStockMutationRepository _mutationRepo;
    private readonly ISupplierRepository _supplierRepo;
    private readonly IProductRepository _productRepo;

    public PurchaseOrderService(
        IPurchaseOrderRepository poRepo,
        IStockRepository stockRepo,
        IStockMutationRepository mutationRepo,
        ISupplierRepository supplierRepo,
        IProductRepository productRepo)
    {
        _poRepo = poRepo;
        _stockRepo = stockRepo;
        _mutationRepo = mutationRepo;
        _supplierRepo = supplierRepo;
        _productRepo = productRepo;
    }

    public async Task<ApiResponse<IEnumerable<PurchaseOrderDto>>> GetAllAsync()
    {
        var orders = await _poRepo.GetAllAsync();
        var dtos = new List<PurchaseOrderDto>();
        foreach (var o in orders)
        {
            var supplier = await _supplierRepo.GetByIdAsync(o.SupplierId);
            var poWithDetails = await _poRepo.GetWithDetailsAsync(o.Id);
            dtos.Add(await MapToDtoAsync(poWithDetails!, supplier?.Name ?? "Unknown"));
        }
        return ApiResponse<IEnumerable<PurchaseOrderDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<PurchaseOrderDto>> GetByIdAsync(int id)
    {
        var po = await _poRepo.GetWithDetailsAsync(id);
        if (po == null)
            return ApiResponse<PurchaseOrderDto>.Fail("Purchase order not found.", "NOT_FOUND");

        var supplier = await _supplierRepo.GetByIdAsync(po.SupplierId);
        return ApiResponse<PurchaseOrderDto>.Ok(await MapToDtoAsync(po, supplier?.Name ?? "Unknown"));
    }

    public async Task<ApiResponse<PurchaseOrderDto>> CreateAsync(CreatePurchaseOrderDto dto)
    {
        var supplier = await _supplierRepo.GetByIdAsync(dto.SupplierId);
        if (supplier == null)
            return ApiResponse<PurchaseOrderDto>.Fail("Supplier not found.", "NOT_FOUND");

        var po = new PurchaseOrder
        {
            SupplierId = dto.SupplierId,
            Status = PurchaseOrderStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var d in dto.Details)
        {
            po.Details.Add(new PODetail
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice
            });
        }

        po.TotalAmount = po.Details.Sum(d => d.Quantity * d.UnitPrice);
        await _poRepo.AddAsync(po);

        return ApiResponse<PurchaseOrderDto>.Ok(await MapToDtoAsync(po, supplier.Name), "Purchase order created.");
    }

    public async Task<ApiResponse<PurchaseOrderDto>> ConfirmAsync(int id)
    {
        var po = await _poRepo.GetWithDetailsAsync(id);
        if (po == null)
            return ApiResponse<PurchaseOrderDto>.Fail("Purchase order not found.", "NOT_FOUND");

        po.Confirm();
        await _poRepo.UpdateAsync(po);

        var supplier = await _supplierRepo.GetByIdAsync(po.SupplierId);
        return ApiResponse<PurchaseOrderDto>.Ok(await MapToDtoAsync(po, supplier?.Name ?? "Unknown"), "Order confirmed.");
    }

    public async Task<ApiResponse<PurchaseOrderDto>> ReceiveOrderAsync(int id)
    {
        var po = await _poRepo.GetWithDetailsAsync(id);
        if (po == null)
            return ApiResponse<PurchaseOrderDto>.Fail("Purchase order not found.", "NOT_FOUND");

        po.MarkReceived();
        await _poRepo.UpdateAsync(po);

        // Auto-increment stock per PODetail
        foreach (var detail in po.Details)
        {
            var stock = await _stockRepo.GetByProductIdAsync(detail.ProductId);
            if (stock == null)
            {
                stock = new Stock { ProductId = detail.ProductId, Quantity = 0, LastUpdated = DateTime.UtcNow };
                await _stockRepo.AddAsync(stock);
            }

            stock.Quantity += detail.Quantity;
            stock.LastUpdated = DateTime.UtcNow;
            await _stockRepo.UpdateAsync(stock);

            await _mutationRepo.AddAsync(new StockMutation
            {
                ProductId = detail.ProductId,
                Type = MutationType.In,
                Quantity = detail.Quantity,
                Note = $"Received from PO #{po.Id}",
                CreatedAt = DateTime.UtcNow
            });
        }

        var supplier = await _supplierRepo.GetByIdAsync(po.SupplierId);
        return ApiResponse<PurchaseOrderDto>.Ok(await MapToDtoAsync(po, supplier?.Name ?? "Unknown"), "Order received and stock updated.");
    }

    public async Task<ApiResponse<PurchaseOrderDto>> CancelAsync(int id)
    {
        var po = await _poRepo.GetWithDetailsAsync(id);
        if (po == null)
            return ApiResponse<PurchaseOrderDto>.Fail("Purchase order not found.", "NOT_FOUND");

        po.Cancel();
        await _poRepo.UpdateAsync(po);

        var supplier = await _supplierRepo.GetByIdAsync(po.SupplierId);
        return ApiResponse<PurchaseOrderDto>.Ok(await MapToDtoAsync(po, supplier?.Name ?? "Unknown"), "Order cancelled.");
    }

    private async Task<PurchaseOrderDto> MapToDtoAsync(PurchaseOrder po, string supplierName)
    {
        var details = new List<PODetailDto>();
        foreach (var d in po.Details)
        {
            var product = await _productRepo.GetByIdAsync(d.ProductId);
            details.Add(new PODetailDto
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductName = product?.Name ?? "Unknown",
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice
            });
        }

        return new PurchaseOrderDto
        {
            Id = po.Id,
            SupplierId = po.SupplierId,
            SupplierName = supplierName,
            Status = po.Status,
            TotalAmount = po.TotalAmount,
            CreatedAt = po.CreatedAt,
            ReceivedAt = po.ReceivedAt,
            Details = details
        };
    }
}
