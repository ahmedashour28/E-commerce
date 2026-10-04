using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Api.Entities.OrderAggregate
{
    [Table("OrderItems")]
    public class OrderItem : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}