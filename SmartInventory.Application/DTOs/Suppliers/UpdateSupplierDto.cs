using System.ComponentModel.DataAnnotations;

namespace SmartInventory.Application.DTOs.Suppliers;

public class UpdateSupplierDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Contact { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
