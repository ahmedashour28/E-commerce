namespace ECommerce.Api.Entities
{
    public class Wishlist : BaseEntity
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}