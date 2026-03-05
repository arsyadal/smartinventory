using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IStockService _stockService;
    private readonly IPurchaseOrderService _poService;
    private readonly IPriceService _priceService;

    public HomeController(
        IProductService productService,
        IStockService stockService,
        IPurchaseOrderService poService,
        IPriceService priceService)
    {
        _productService = productService;
        _stockService = stockService;
        _poService = poService;
        _priceService = priceService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        var lowStock = await _stockService.GetLowStockAsync();
        var orders = await _poService.GetAllAsync();
        var prices = await _priceService.GetByKeywordAsync("laptop");

        ViewBag.TotalProducts = products.Data?.Count() ?? 0;
        ViewBag.LowStockCount = lowStock.Data?.Count() ?? 0;
        ViewBag.ActivePOs = orders.Data?.Count(o =>
            o.Status == Domain.Enums.PurchaseOrderStatus.Draft ||
            o.Status == Domain.Enums.PurchaseOrderStatus.Confirmed) ?? 0;
        ViewBag.LastPriceUpdate = prices.Data?.FirstOrDefault()?.ScrapedAt.ToString("dd MMM HH:mm") ?? "N/A";
        ViewBag.LowStockItems = lowStock.Data?.Take(5).ToList();

        // Chart data: mutations last 7 days
        var stocks = await _stockService.GetAllAsync();
        ViewBag.StockData = stocks.Data?.Select(s => new { s.ProductName, s.Quantity }).Take(7).ToList();

        return View();
    }
}
