using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.DTOs.PurchaseOrders;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Web.Controllers;

[Authorize]
public class PurchaseOrderController : Controller
{
    private readonly IPurchaseOrderService _poService;
    private readonly ISupplierService _supplierService;
    private readonly IProductService _productService;

    public PurchaseOrderController(
        IPurchaseOrderService poService,
        ISupplierService supplierService,
        IProductService productService)
    {
        _poService = poService;
        _supplierService = supplierService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _poService.GetAllAsync();
        return View(result.Data ?? Enumerable.Empty<PurchaseOrderDto>());
    }

    public async Task<IActionResult> Create()
    {
        var suppliers = await _supplierService.GetAllAsync();
        var products = await _productService.GetAllAsync();
        ViewBag.Suppliers = suppliers.Data ?? Enumerable.Empty<Application.DTOs.Suppliers.SupplierDto>();
        ViewBag.Products = products.Data ?? Enumerable.Empty<Application.DTOs.Products.ProductDto>();
        return View(new CreatePurchaseOrderDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
    {
        if (!ModelState.IsValid || dto.Details.Count == 0)
        {
            var suppliers = await _supplierService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            ViewBag.Suppliers = suppliers.Data ?? Enumerable.Empty<Application.DTOs.Suppliers.SupplierDto>();
            ViewBag.Products = products.Data ?? Enumerable.Empty<Application.DTOs.Products.ProductDto>();
            ModelState.AddModelError("", "At least one product detail is required.");
            return View(dto);
        }

        var result = await _poService.CreateAsync(dto);
        TempData["Success"] = "Purchase order created.";
        return RedirectToAction(nameof(Details), new { id = result.Data?.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _poService.GetByIdAsync(id);
        if (!result.Success) return NotFound();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id)
    {
        var result = await _poService.ConfirmAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(int id)
    {
        var result = await _poService.ReceiveOrderAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _poService.CancelAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }
}
