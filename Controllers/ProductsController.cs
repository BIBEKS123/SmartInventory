using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Data;
using SmartInventory.Models;
using Microsoft.AspNetCore.Authorization;

namespace SmartInventory.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
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

            // Product image upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string[] allowedExtensions =
                    { ".jpg", ".jpeg", ".png", ".webp" };

                string extension =
                    Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "", "Only JPG, JPEG, PNG and WEBP images are allowed.");

                    return View(product);
                }

                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "", "Image size must be less than 5 MB.");

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

            // Save product directly into SQL Server
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Products");
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Stock = product.Stock;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}