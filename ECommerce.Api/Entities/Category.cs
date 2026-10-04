using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
