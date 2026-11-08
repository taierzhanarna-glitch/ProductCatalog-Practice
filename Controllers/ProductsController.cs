using Microsoft.AspNetCore.Mvc;
using ProductCatalogPractice.Services;

namespace ProductCatalogPractice.Controllers;

public sealed class ProductsController(IProductCatalog catalog) : Controller
{
    [HttpGet]
    public IActionResult Create()
    {
        PopulateCategories();
        ViewData["Title"] = "Жаңа өнім";
        return View(new ProductCatalogPractice.Models.Product());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(ProductCatalogPractice.Models.Product product)
    {
        product.Name = product.Name?.Trim() ?? string.Empty;
        product.Category = product.Category?.Trim() ?? string.Empty;
        product.Description = product.Description?.Trim() ?? string.Empty;
        product.ImageUrl = product.ImageUrl?.Trim();

        if (!string.IsNullOrWhiteSpace(product.ImageUrl) && !IsAllowedImageUrl(product.ImageUrl))
            ModelState.AddModelError(nameof(product.ImageUrl), "HTTPS арқылы ашылатын сурет сілтемесін енгізіңіз.");

        if (!ModelState.IsValid)
        {
            PopulateCategories();
            ViewData["Title"] = "Жаңа өнім";
            return View(product);
        }

        catalog.Add(product);
        TempData["Notice"] = "Өнім каталогқа қосылды.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index(string? q, string? category, string? sort = "featured")
    {
        var products = catalog.GetAll();
        ViewBag.Categories = products.Select(product => product.Category).Distinct().Order().ToList();
        ViewBag.SelectedCategory = category ?? string.Empty;
        ViewBag.SearchTerm = q ?? string.Empty;
        ViewBag.SelectedSort = sort ?? "featured";
        ViewBag.Notice = TempData["Notice"];
        ViewData["Title"] = "Өнімдер каталогы";

        IEnumerable<ProductCatalogPractice.Models.Product> result = products;

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            result = result.Where(product => product.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || product.Category.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category) && products.Any(product => product.Category == category))
            result = result.Where(product => product.Category == category);

        result = sort switch
        {
            "price-asc" => result.OrderBy(product => product.Price),
            "price-desc" => result.OrderByDescending(product => product.Price),
            "name" => result.OrderBy(product => product.Name),
            _ => result.OrderBy(product => product.Id)
        };

        var filteredProducts = result.ToArray();
        ViewBag.TotalCount = filteredProducts.Length;

        return View(filteredProducts);
    }

    public IActionResult Details(int id)
    {
        var product = catalog.GetById(id);
        return product is null ? NotFound() : View(product);
    }

    private void PopulateCategories() =>
        ViewBag.Categories = catalog.GetAll().Select(product => product.Category).Distinct().Order().ToList();

    private static bool IsAllowedImageUrl(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && uri.Scheme == Uri.UriSchemeHttps
        && !string.IsNullOrWhiteSpace(uri.Host);
}
