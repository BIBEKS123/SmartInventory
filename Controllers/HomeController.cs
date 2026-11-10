using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Data;
using System.Net.Http.Json;
using SmartInventory.Models;
using Microsoft.AspNetCore.Authorization;

namespace SmartInventory.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public HomeController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7178/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalProducts = await _context.Products.CountAsync();

            ViewBag.LowStock = await _context.Products
                .CountAsync(p => p.Stock <= 5);

            return View();

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync();

            return View(products);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(
               Product product,
               IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Photo upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

                string extension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ImageFile", "Only JPG, PNG and WEBP images are allowed.");

                    return View(product);
                }

                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ImageFile", "Image size must be less than 5 MB.");

                    return View(product);
                }

                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads");

                Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + extension;

                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImagePath = "/uploads/" + fileName;
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Products");


        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var existingProduct = await _context.Products.FindAsync(product.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.Category = product.Category;
            existingProduct.Description = product.Description;
            existingProduct.SKU = product.SKU;
            existingProduct.SupplierName = product.SupplierName;
            existingProduct.PurchaseDate = product.PurchaseDate;

            await _context.SaveChangesAsync();

            return RedirectToAction("Products");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction("Products");
        }
        public IActionResult Chatbot()
        {
            return View();
        }
    }

}    
