using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Entities
{
    public class Review : BaseEntity
    {
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}