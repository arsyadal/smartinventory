using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.DTOs.Stocks;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Web.Controllers;

[Authorize]
public class StockController : Controller
{
    private readonly IStockService _stockService;
    private readonly IProductService _productService;

    public StockController(IStockService stockService, IProductService productService)
    {
        _stockService = stockService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _stockService.GetAllAsync();
        return View(result.Data ?? Enumerable.Empty<StockDto>());
    }

    public async Task<IActionResult> Mutation()
    {
        var products = await _productService.GetAllAsync();
        ViewBag.Products = products.Data ?? Enumerable.Empty<Application.DTOs.Products.ProductDto>();
        return View(new CreateMutationDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Mutation(CreateMutationDto dto)
    {
        if (!ModelState.IsValid)
        {
            var products = await _productService.GetAllAsync();
            ViewBag.Products = products.Data ?? Enumerable.Empty<Application.DTOs.Products.ProductDto>();
            return View(dto);
        }

        var result = await _stockService.AddMutationAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            var products = await _productService.GetAllAsync();
            ViewBag.Products = products.Data ?? Enumerable.Empty<Application.DTOs.Products.ProductDto>();
            return View(dto);
        }

        TempData["Success"] = "Stock mutation recorded successfully.";
        return RedirectToAction(nameof(Index));
    }
}
