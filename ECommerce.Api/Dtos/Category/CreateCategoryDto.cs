namespace ECommerce.Api.Dtos.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }
}
