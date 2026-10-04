namespace ECommerce.Api.Entities
{
    public class Cart : BaseEntity
    {
        public string BuyerId { get; set; } = string.Empty;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(Product product, int quantity)
        {
            if (Items.All(item => item.ProductId != product.Id))
            {
                Items.Add(new CartItem { Product = product, Quantity = quantity });
                return;
            }

            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null) existingItem.Quantity += quantity;
        }

        public void RemoveItem(Guid productId, int quantity)
        {
            var item = Items.FirstOrDefault(item => item.ProductId == productId);
            if (item == null) return;

            item.Quantity -= quantity;
            if (item.Quantity <= 0) Items.Remove(item);
        }
    }
}
