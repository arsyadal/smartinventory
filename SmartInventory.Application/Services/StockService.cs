using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Stocks;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;
using SmartInventory.Domain.Exceptions;

namespace SmartInventory.Application.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepo;
    private readonly IStockMutationRepository _mutationRepo;
    private readonly IProductRepository _productRepo;

    public StockService(IStockRepository stockRepo, IStockMutationRepository mutationRepo, IProductRepository productRepo)
    {
        _stockRepo = stockRepo;
        _mutationRepo = mutationRepo;
        _productRepo = productRepo;
    }

    public async Task<ApiResponse<IEnumerable<StockDto>>> GetAllAsync()
    {
        var stocks = await _stockRepo.GetAllAsync();
        var dtos = new List<StockDto>();
        foreach (var s in stocks)
        {
            var product = await _productRepo.GetByIdAsync(s.ProductId);
            if (product == null) continue;
            dtos.Add(MapToDto(s, product));
        }
        return ApiResponse<IEnumerable<StockDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<IEnumerable<LowStockDto>>> GetLowStockAsync()
    {
        var lowStock = await _stockRepo.GetLowStockAsync();
        return ApiResponse<IEnumerable<LowStockDto>>.Ok(lowStock);
    }

    public async Task<ApiResponse<StockMutationDto>> AddMutationAsync(CreateMutationDto dto)
    {
        var product = await _productRepo.GetByIdAsync(dto.ProductId);
        if (product == null)
            return ApiResponse<StockMutationDto>.Fail("Product not found.", "NOT_FOUND");

        var stock = await _stockRepo.GetByProductIdAsync(dto.ProductId);
        if (stock == null)
        {
            stock = new Stock { ProductId = dto.ProductId, Quantity = 0, LastUpdated = DateTime.UtcNow };
            await _stockRepo.AddAsync(stock);
        }

        if (dto.Type == MutationType.Out && stock.Quantity < dto.Quantity)
            throw new InsufficientStockException(product.Name, dto.Quantity, stock.Quantity);

        if (dto.Type == MutationType.In)
            stock.Quantity += dto.Quantity;
        else
            stock.Quantity -= dto.Quantity;

        stock.LastUpdated = DateTime.UtcNow;
        await _stockRepo.UpdateAsync(stock);

        var mutation = new StockMutation
        {
            ProductId = dto.ProductId,
            Type = dto.Type,
            Quantity = dto.Quantity,
            Note = dto.Note,
            CreatedAt = DateTime.UtcNow
        };
        await _mutationRepo.AddAsync(mutation);

        return ApiResponse<StockMutationDto>.Ok(MapMutationToDto(mutation, product.Name), "Mutation recorded successfully.");
    }

    public async Task<ApiResponse<IEnumerable<StockMutationDto>>> GetMutationsByProductAsync(int productId)
    {
        var mutations = await _mutationRepo.GetByProductIdAsync(productId);
        var product = await _productRepo.GetByIdAsync(productId);
        var productName = product?.Name ?? "Unknown";

        var dtos = mutations.Select(m => MapMutationToDto(m, productName));
        return ApiResponse<IEnumerable<StockMutationDto>>.Ok(dtos);
    }

    private static StockDto MapToDto(Stock s, Domain.Entities.Product p) => new()
    {
        Id = s.Id,
        ProductId = s.ProductId,
        ProductName = p.Name,
        SKU = p.SKU,
        Quantity = s.Quantity,
        MinStock = p.MinStock,
        LastUpdated = s.LastUpdated
    };

    private static StockMutationDto MapMutationToDto(StockMutation m, string productName) => new()
    {
        Id = m.Id,
        ProductId = m.ProductId,
        ProductName = productName,
        Type = m.Type,
        Quantity = m.Quantity,
        Note = m.Note,
        CreatedAt = m.CreatedAt
    };
}
