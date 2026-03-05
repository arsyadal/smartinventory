using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Prices;

namespace SmartInventory.Application.Interfaces.Services;

public interface IPriceService
{
    Task<ApiResponse<IEnumerable<PriceHistoryDto>>> GetByKeywordAsync(string keyword);
    Task<ApiResponse<IEnumerable<PriceTrendDto>>> GetTrendAsync(string keyword);
}
