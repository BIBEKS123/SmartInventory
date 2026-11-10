using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartInventory.Data;
using SmartInventory.Models;

namespace SmartInventory.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public SalesController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Sales History
        public async Task<IActionResult> Index(
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = _context.Sales
                .Include(s => s.Product)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(s =>
                    s.SaleDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var endExclusive = endDate.Value.Date.AddDays(1);

                query = query.Where(s =>
                    s.SaleDate < endExclusive);
            }

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            var sales = await query
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return View(sales);
        }
        // GET: Sales/Invoice/5
        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }
        // New Sale Page
        public async Task<IActionResult> Create()
        {
            await LoadProducts();
            return View();
        }

        // Save New Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale)
        {
            ModelState.Remove(nameof(Sale.Product));
            ModelState.Remove(nameof(Sale.UnitPrice));

            if (!ModelState.IsValid)
            {
                await LoadProducts();
                return View(sale);
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == sale.ProductId);

            if (product == null)
            {
                ModelState.AddModelError("", "Product not found.");
                await LoadProducts();
                return View(sale);
            }

            if (sale.Quantity < 1)
            {
                ModelState.AddModelError(
                    "Quantity",
                    "Quantity must be at least 1.");

                await LoadProducts();
                return View(sale);
            }

            if (sale.Quantity > product.Stock)
            {
                ModelState.AddModelError(
                    "Quantity",
                    "Available stock is only " + product.Stock);

                await LoadProducts();
                return View(sale);
            }

            sale.UnitPrice = product.Price;
            sale.TotalAmount = product.Price * sale.Quantity;
            sale.SaleDate = DateTime.Now;

            // Sold quantity ko stock se minus karo
            product.Stock -= sale.Quantity;

            _context.Sales.Add(sale);

            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] = "Sale saved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Delete One Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id,
            string adminDeleteCode)
        {
            if (!IsDeleteCodeValid(adminDeleteCode))
            {
                TempData["DeleteError"] =
                    "Invalid Admin Delete Code.";

                return RedirectToAction(nameof(Index));
            }

            var sale = await _context.Sales.FindAsync(id);

            if (sale == null)
            {
                TempData["DeleteError"] =
                    "Sale record not found.";

                return RedirectToAction(nameof(Index));
            }

            // Sirf sale delete hogi.
            // Product stock change nahi hoga.
            _context.Sales.Remove(sale);

            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] =
                "Sale deleted successfully. Product stock was not changed.";

            return RedirectToAction(nameof(Index));
        }

        // Delete All Sales
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(
            string adminDeleteCode)
        {
            if (!IsDeleteCodeValid(adminDeleteCode))
            {
                TempData["DeleteError"] =
                    "Invalid Admin Delete Code.";

                return RedirectToAction(nameof(Index));
            }

            var allSales = await _context.Sales.ToListAsync();

            if (allSales.Count == 0)
            {
                TempData["DeleteError"] =
                    "There are no sales records to delete.";

                return RedirectToAction(nameof(Index));
            }

            // Sirf sales records delete honge.
            // Products aur unka stock change nahi hoga.
            _context.Sales.RemoveRange(allSales);

            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] =
                "All sales records deleted. Product stock was not changed.";

            return RedirectToAction(nameof(Index));
        }

        // Admin Delete Code Check
        private bool IsDeleteCodeValid(string adminDeleteCode)
        {
            var savedCode = _configuration["AdminDeleteCode"];

            return !string.IsNullOrWhiteSpace(savedCode)
                && !string.IsNullOrWhiteSpace(adminDeleteCode)
                && string.Equals(
                    adminDeleteCode,
                    savedCode,
                    StringComparison.Ordinal);
        }

        // Product Dropdown
        private async Task LoadProducts()
        {
            var products = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();

            ViewBag.Products = new SelectList(
                products.Select(p => new
                {
                    Id = p.Id,
                    DisplayName = p.Name +
                        " (Stock: " + p.Stock + ")"
                }),
                "Id",
                "DisplayName"
            );
        }
    }
}