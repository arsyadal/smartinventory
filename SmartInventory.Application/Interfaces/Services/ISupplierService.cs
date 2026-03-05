using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Suppliers;

namespace SmartInventory.Application.Interfaces.Services;

public interface ISupplierService
{
    Task<ApiResponse<IEnumerable<SupplierDto>>> GetAllAsync();
    Task<ApiResponse<SupplierDto>> GetByIdAsync(int id);
    Task<ApiResponse<SupplierDto>> CreateAsync(CreateSupplierDto dto);
    Task<ApiResponse<SupplierDto>> UpdateAsync(int id, UpdateSupplierDto dto);
    Task<ApiResponse> DeleteAsync(int id);
}
