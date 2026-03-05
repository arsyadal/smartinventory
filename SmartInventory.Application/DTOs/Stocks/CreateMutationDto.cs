using System.ComponentModel.DataAnnotations;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.DTOs.Stocks;

public class CreateMutationDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public MutationType Type { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}
