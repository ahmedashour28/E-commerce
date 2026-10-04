using ECommerce.Api.Entities.OrderAggregate;
using ECommerce.Api.Repositories.Interfaces;

namespace ECommerce.Api.Services.Background
{
    public class StockReleaseService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StockReleaseService> _logger;

        public StockReleaseService(IServiceProvider serviceProvider, ILogger<StockReleaseService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredOrders(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing expired orders");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task ProcessExpiredOrders(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            var cutoffTime = DateTime.UtcNow.AddMinutes(-30);
            var expiredOrders = await orderRepo.GetExpiredOrdersAsync(cutoffTime);

            if (expiredOrders.Any())
            {
                _logger.LogInformation("Found {Count} expired orders to cancel.", expiredOrders.Count);

                foreach (var order in expiredOrders)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var product = await productRepo.GetByIdAsync(item.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity += item.Quantity;
                            await productRepo.UpdateAsync(product);
                        }
                    }

                    order.Status = OrderStatus.PaymentFailed;
                    await orderRepo.UpdateAsync(order);

                    _logger.LogInformation("Order {OrderId} cancelled and stock released.", order.Id);
                }
            }
        }
    }
}
