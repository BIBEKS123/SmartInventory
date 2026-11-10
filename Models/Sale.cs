using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SmartInventory.Models
{
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        [Precision(18, 2)]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal UnitPrice { get; set; }

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public string? CustomerName { get; set; }
        [MaxLength(15)]
        public string? CustomerPhone { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;
    }
}