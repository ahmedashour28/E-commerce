namespace ECommerce.Api.Dtos.Product
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
