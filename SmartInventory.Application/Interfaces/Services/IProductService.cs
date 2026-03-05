using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Products;

namespace SmartInventory.Application.Interfaces.Services;

public interface IProductService
{
    Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync();
    Task<ApiResponse<ProductDto>> GetByIdAsync(int id);
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto);
    Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto dto);
    Task<ApiResponse> DeleteAsync(int id);
}
