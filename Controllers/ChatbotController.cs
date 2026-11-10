using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartInventory.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class ChatbotController : ControllerBase
        {
            private readonly AppDbContext _context;

            public ChatbotController(AppDbContext context)
            {
                _context = context;
            }

            [HttpPost]
            public async Task<IActionResult> Chat(
                [FromBody] ChatRequest request)
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                    return BadRequest("Please enter a question.");

                var message = request.Message.ToLowerInvariant();
              
               var product = await _context.Products .FirstOrDefaultAsync(p => message.Contains(p.Name.ToLower()));

            if (product != null)
            {
                return Ok(new
                {
                    reply = $"Available stock of {product.Name} is {product.Stock} items."
                });
            }

            if (message.Contains("total") ||
                    message.Contains("how many products"))
                {
                    var count = await _context.Products.CountAsync();

                    return Ok(new
                    {
                        reply = $"You have {count} products in inventory."
                    });
                }

                if (message.Contains("low stock") ||
                    message.Contains("out of stock"))
                {
                    var products = await _context.Products
                        .Where(p => p.Stock <= 5)
                        .Select(p => new { p.Name, p.Stock })
                        .ToListAsync();

                    return Ok(new
                    {
                        reply = products.Count == 0
                            ? "No low-stock products found."
                            : "Low-stock products: " +
                              string.Join(", ", products.Select(
                                  p => $"{p.Name} ({p.Stock} left)"))
                    });
                }

                return Ok(new
                {
                    reply = "Ask me about total products or low stock."
                });
            }
        }

        public class ChatRequest
        {
            public string Message { get; set; } = string.Empty;
        }
}


