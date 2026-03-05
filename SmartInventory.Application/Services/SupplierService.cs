using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Suppliers;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepo;

    public SupplierService(ISupplierRepository supplierRepo)
    {
        _supplierRepo = supplierRepo;
    }

    public async Task<ApiResponse<IEnumerable<SupplierDto>>> GetAllAsync()
    {
        var suppliers = await _supplierRepo.GetAllAsync();
        return ApiResponse<IEnumerable<SupplierDto>>.Ok(suppliers.Select(MapToDto));
    }

    public async Task<ApiResponse<SupplierDto>> GetByIdAsync(int id)
    {
        var supplier = await _supplierRepo.GetByIdAsync(id);
        if (supplier == null)
            return ApiResponse<SupplierDto>.Fail("Supplier not found.", "NOT_FOUND");
        return ApiResponse<SupplierDto>.Ok(MapToDto(supplier));
    }

    public async Task<ApiResponse<SupplierDto>> CreateAsync(CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
            Contact = dto.Contact,
            Address = dto.Address,
            IsActive = true
        };
        await _supplierRepo.AddAsync(supplier);
        return ApiResponse<SupplierDto>.Ok(MapToDto(supplier), "Supplier created successfully.");
    }

    public async Task<ApiResponse<SupplierDto>> UpdateAsync(int id, UpdateSupplierDto dto)
    {
        var supplier = await _supplierRepo.GetByIdAsync(id);
        if (supplier == null)
            return ApiResponse<SupplierDto>.Fail("Supplier not found.", "NOT_FOUND");

        supplier.Name = dto.Name;
        supplier.Contact = dto.Contact;
        supplier.Address = dto.Address;
        supplier.IsActive = dto.IsActive;

        await _supplierRepo.UpdateAsync(supplier);
        return ApiResponse<SupplierDto>.Ok(MapToDto(supplier), "Supplier updated successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(int id)
    {
        var supplier = await _supplierRepo.GetByIdAsync(id);
        if (supplier == null)
            return ApiResponse.Fail("Supplier not found.", "NOT_FOUND");

        supplier.IsActive = false;
        await _supplierRepo.UpdateAsync(supplier);
        return ApiResponse.Ok("Supplier deleted successfully.");
    }

    private static SupplierDto MapToDto(Supplier s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Contact = s.Contact,
        Address = s.Address,
        IsActive = s.IsActive
    };
}
