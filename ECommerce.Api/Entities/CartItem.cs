namespace ECommerce.Api.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;
    }
}
