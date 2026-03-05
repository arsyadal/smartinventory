using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Prices;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Application.Services;

public class PriceService : IPriceService
{
    private readonly IPriceHistoryRepository _priceRepo;

    public PriceService(IPriceHistoryRepository priceRepo)
    {
        _priceRepo = priceRepo;
    }

    public async Task<ApiResponse<IEnumerable<PriceHistoryDto>>> GetByKeywordAsync(string keyword)
    {
        var records = await _priceRepo.GetByKeywordAsync(keyword);
        var dtos = records.Select(r => new PriceHistoryDto
        {
            Id = r.Id,
            ProductName = r.ProductName,
            Price = r.Price,
            Source = r.Source,
            ScrapedAt = r.ScrapedAt
        });
        return ApiResponse<IEnumerable<PriceHistoryDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<IEnumerable<PriceTrendDto>>> GetTrendAsync(string keyword)
    {
        var records = await _priceRepo.GetByKeywordAsync(keyword);

        var trend = records
            .GroupBy(r => r.ScrapedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new PriceTrendDto
            {
                ProductName = keyword,
                Date = g.Key,
                AvgPrice = g.Average(r => r.Price),
                MinPrice = g.Min(r => r.Price),
                MaxPrice = g.Max(r => r.Price)
            });

        return ApiResponse<IEnumerable<PriceTrendDto>>.Ok(trend);
    }
}
