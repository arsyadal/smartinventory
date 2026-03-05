using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Stocks;

namespace SmartInventory.Application.Interfaces.Services;

public interface IStockService
{
    Task<ApiResponse<IEnumerable<StockDto>>> GetAllAsync();
    Task<ApiResponse<IEnumerable<LowStockDto>>> GetLowStockAsync();
    Task<ApiResponse<StockMutationDto>> AddMutationAsync(CreateMutationDto dto);
    Task<ApiResponse<IEnumerable<StockMutationDto>>> GetMutationsByProductAsync(int productId);
}
