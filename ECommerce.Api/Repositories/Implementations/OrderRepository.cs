using ECommerce.Api.Data;
using ECommerce.Api.Entities.OrderAggregate;
using ECommerce.Api.Helpers;
using ECommerce.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext _context;

        public OrderRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id, string buyerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.BuyerId == buyerId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string buyerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedList<Order>> GetAllOrdersAsync(OrderParams orderParams)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(orderParams.Status) && Enum.TryParse<OrderStatus>(orderParams.Status, true, out var statusEnum))
            {
                query = query.Where(o => o.Status == statusEnum);
            }

            return await PagedList<Order>.CreateAsync(
                query,
                orderParams.PageNumber,
                orderParams.PageSize
            );
        }

        public async Task<Order?> GetOrderByIdAdminAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(string buyerId, Guid productId)
        {
            var validStatuses = new[]
            {
                OrderStatus.PaymentReceived,
                OrderStatus.Shipped,
                OrderStatus.Complete
            };

            return await _context.Orders
                .Where(o => o.BuyerId == buyerId && validStatuses.Contains(o.Status))
                .SelectMany(o => o.OrderItems)
                .AnyAsync(oi => oi.ProductId == productId);
        }

        public async Task<Order?> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
        }

        public async Task<List<Order>> GetExpiredOrdersAsync(DateTime olderThan)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o =>
                    o.Status == OrderStatus.Pending &&
                    o.PaymentMethod == PaymentMethod.Stripe &&
                    o.CreatedAt < olderThan)
                .ToListAsync();
        }
    }
}