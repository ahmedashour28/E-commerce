namespace ECommerce.Api.Dtos.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ParentCategoryName { get; set; }
    }
}
