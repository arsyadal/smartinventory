using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.DTOs.Products;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Web.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService) => _productService = productService;

    public async Task<IActionResult> Index()
    {
        var result = await _productService.GetAllAsync();
        return View(result.Data ?? Enumerable.Empty<ProductDto>());
    }

    public IActionResult Create() => View(new CreateProductDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _productService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            return View(dto);
        }

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (!result.Success) return NotFound();

        var dto = new UpdateProductDto
        {
            Name = result.Data!.Name,
            SKU = result.Data.SKU,
            Category = result.Data.Category,
            Price = result.Data.Price,
            MinStock = result.Data.MinStock,
            IsActive = result.Data.IsActive
        };
        ViewBag.ProductId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ProductId = id;
            return View(dto);
        }

        var result = await _productService.UpdateAsync(id, dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            ViewBag.ProductId = id;
            return View(dto);
        }

        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (!result.Success) return NotFound();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }
}
