using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Api.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public required string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
