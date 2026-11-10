
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SmartInventory.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Precision(18, 2)]
        [Range(typeof(decimal), "0.01", "999999999",
            ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue,
            ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string? Category { get; set; }

        public string? Description { get; set; }

        public string? SKU { get; set; }

        public string? SupplierName { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string? ImagePath { get; set; }
    }
}



