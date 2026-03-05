using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.API.Controllers;

[ApiController]
[Route("api/prices")]
[Authorize]
public class PricesController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PricesController(IPriceService priceService) => _priceService = priceService;

    [HttpGet("{keyword}")]
    public async Task<IActionResult> GetByKeyword(string keyword) =>
        Ok(await _priceService.GetByKeywordAsync(keyword));

    [HttpGet("{keyword}/trend")]
    public async Task<IActionResult> GetTrend(string keyword) =>
        Ok(await _priceService.GetTrendAsync(keyword));
}
