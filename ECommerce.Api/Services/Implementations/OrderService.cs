using AutoMapper;
using ECommerce.Api.Dtos.Order;
using ECommerce.Api.Entities.OrderAggregate;
using ECommerce.Api.Helpers;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Interfaces;

namespace ECommerce.Api.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepo;

        public OrderService(IOrderRepository orderRepo, ICartRepository cartRepo, IProductRepository productRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(string buyerId, CreateOrderDto dto)
        {
            var cart = await _cartRepo.GetCartAsync(buyerId);
            if (cart == null || !cart.Items.Any()) throw new Exception("Cart is empty");

            var orderItems = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product == null) throw new Exception($"Product {item.ProductId} not found");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Insufficient stock for {product.Name}");

                product.StockQuantity -= item.Quantity;
                await _productRepo.UpdateAsync(product);

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    PictureUrl = product.Images.FirstOrDefault(x => x.IsMain)?.Url ?? "",
                    Price = product.Price,
                    Quantity = item.Quantity
                };
                orderItems.Add(orderItem);
            }

            var address = _mapper.Map<Address>(dto.ShippingAddress);
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            var order = new Order
            {
                BuyerId = buyerId,
                OrderItems = orderItems,
                Subtotal = subtotal,
                ShippingAddress = address,
                Status = OrderStatus.Pending,
                PaymentMethod = dto.PaymentMethod
            };

            await _orderRepo.CreateOrderAsync(order);
            await _cartRepo.DeleteCartAsync(buyerId);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id, string buyerId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id, buyerId);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string buyerId)
        {
            var orders = await _orderRepo.GetOrdersByUserAsync(buyerId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<PagedList<OrderDto>> GetAllOrdersAsync(OrderParams orderParams)
        {
            var pagedOrders = await _orderRepo.GetAllOrdersAsync(orderParams);

            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(pagedOrders);

            return new PagedList<OrderDto>(
                orderDtos.ToList(),
                pagedOrders.TotalCount,
                pagedOrders.CurrentPage,
                pagedOrders.PageSize
            );
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, string status)
        {
            var order = await _orderRepo.GetOrderByIdAdminAsync(orderId);
            if (order == null) throw new Exception("Order not found");

            if (Enum.TryParse<OrderStatus>(status, true, out var result))
            {
                order.Status = result;
                await _orderRepo.UpdateAsync(order);
            }
        }
    }
}