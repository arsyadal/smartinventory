using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Products;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Exceptions;

namespace SmartInventory.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IStockRepository _stockRepo;

    public ProductService(IProductRepository productRepo, IStockRepository stockRepo)
    {
        _productRepo = productRepo;
        _stockRepo = stockRepo;
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync()
    {
        var products = await _productRepo.GetAllAsync();
        var dtos = new List<ProductDto>();
        foreach (var p in products)
        {
            var stock = await _stockRepo.GetByProductIdAsync(p.Id);
            dtos.Add(MapToDto(p, stock?.Quantity ?? 0));
        }
        return ApiResponse<IEnumerable<ProductDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<ProductDto>> GetByIdAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
            return ApiResponse<ProductDto>.Fail("Product not found.", "NOT_FOUND");

        var stock = await _stockRepo.GetByProductIdAsync(id);
        return ApiResponse<ProductDto>.Ok(MapToDto(product, stock?.Quantity ?? 0));
    }

    public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        if (await _productRepo.SkuExistsAsync(dto.SKU))
            throw new DuplicateSkuException(dto.SKU);

        var product = new Product
        {
            Name = dto.Name,
            SKU = dto.SKU,
            Category = dto.Category,
            Price = dto.Price,
            MinStock = dto.MinStock,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepo.AddAsync(product);

        var stock = new Stock { ProductId = product.Id, Quantity = 0, LastUpdated = DateTime.UtcNow };
        await _stockRepo.AddAsync(stock);

        return ApiResponse<ProductDto>.Ok(MapToDto(product, 0), "Product created successfully.");
    }

    public async Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
            return ApiResponse<ProductDto>.Fail("Product not found.", "NOT_FOUND");

        if (await _productRepo.SkuExistsAsync(dto.SKU, excludeId: id))
            throw new DuplicateSkuException(dto.SKU);

        product.Name = dto.Name;
        product.SKU = dto.SKU;
        product.Category = dto.Category;
        product.Price = dto.Price;
        product.MinStock = dto.MinStock;
        product.IsActive = dto.IsActive;

        await _productRepo.UpdateAsync(product);

        var stock = await _stockRepo.GetByProductIdAsync(id);
        return ApiResponse<ProductDto>.Ok(MapToDto(product, stock?.Quantity ?? 0), "Product updated successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
            return ApiResponse.Fail("Product not found.", "NOT_FOUND");

        product.IsActive = false;
        await _productRepo.UpdateAsync(product);

        return ApiResponse.Ok("Product deleted successfully.");
    }

    private static ProductDto MapToDto(Product p, int currentStock) => new()
    {
        Id = p.Id,
        Name = p.Name,
        SKU = p.SKU,
        Category = p.Category,
        Price = p.Price,
        MinStock = p.MinStock,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        CurrentStock = currentStock
    };
}
