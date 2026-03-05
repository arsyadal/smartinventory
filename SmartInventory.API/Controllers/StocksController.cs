using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.API.Controllers;

[ApiController]
[Route("api/stocks")]
[Authorize]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService) => _stockService = stockService;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _stockService.GetAllAsync());

    [HttpGet("lowstock")]
    public async Task<IActionResult> GetLowStock() => Ok(await _stockService.GetLowStockAsync());
}
