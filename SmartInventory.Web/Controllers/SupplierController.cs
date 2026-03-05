using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.DTOs.Suppliers;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Web.Controllers;

[Authorize]
public class SupplierController : Controller
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService) => _supplierService = supplierService;

    public async Task<IActionResult> Index()
    {
        var result = await _supplierService.GetAllAsync();
        return View(result.Data ?? Enumerable.Empty<SupplierDto>());
    }

    public IActionResult Create() => View(new CreateSupplierDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupplierDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _supplierService.CreateAsync(dto);
        TempData["Success"] = "Supplier created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _supplierService.GetByIdAsync(id);
        if (!result.Success) return NotFound();

        var dto = new UpdateSupplierDto
        {
            Name = result.Data!.Name,
            Contact = result.Data.Contact,
            Address = result.Data.Address,
            IsActive = result.Data.IsActive
        };
        ViewBag.SupplierId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateSupplierDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SupplierId = id;
            return View(dto);
        }

        await _supplierService.UpdateAsync(id, dto);
        TempData["Success"] = "Supplier updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _supplierService.DeleteAsync(id);
        TempData["Success"] = "Supplier deleted.";
        return RedirectToAction(nameof(Index));
    }
}
