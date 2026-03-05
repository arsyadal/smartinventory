using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Web.Controllers;

[Authorize]
public class PriceMonitorController : Controller
{
    private readonly IPriceService _priceService;

    public PriceMonitorController(IPriceService priceService) => _priceService = priceService;

    public async Task<IActionResult> Index(string keyword = "laptop gaming")
    {
        var history = await _priceService.GetByKeywordAsync(keyword);
        var trend = await _priceService.GetTrendAsync(keyword);

        ViewBag.Keyword = keyword;
        ViewBag.TrendData = trend.Data?.ToList();

        return View(history.Data ?? Enumerable.Empty<Application.DTOs.Prices.PriceHistoryDto>());
    }
}
