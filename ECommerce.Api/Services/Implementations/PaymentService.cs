using ECommerce.Api.Entities;
using ECommerce.Api.Entities.OrderAggregate;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Interfaces;
using ECommerce.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Api.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IOrderRepository _orderRepo;
        private readonly ILogger<PaymentService> _logger;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentService(
            IOptions<StripeSettings> stripeSettings,
            IOrderRepository orderRepo,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            ILogger<PaymentService> logger)
        {
            _stripeSettings = stripeSettings.Value;
            _orderRepo = orderRepo;
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<Order> CreateOrUpdatePaymentIntentAsync(Guid orderId)
        {
            var order = await _orderRepo.GetOrderByIdAdminAsync(orderId);

            if (order == null) throw new Exception("Order not found");

            if (order.Status == OrderStatus.PaymentReceived || order.Status == OrderStatus.Complete)
                throw new Exception("Order is already paid");

            var service = new PaymentIntentService();
            PaymentIntent intent;

            var amount = (long)(order.GetTotal() * 100);

            if (string.IsNullOrEmpty(order.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderId", order.Id.ToString() }
                    }
                };
                intent = await service.CreateAsync(options);
                order.PaymentIntentId = intent.Id;
                order.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = amount
                };
                intent = await service.UpdateAsync(order.PaymentIntentId, options);
            }

            await _orderRepo.UpdateAsync(order);
            return order;
        }

        public async Task UpdateOrderPaymentStatus(string paymentIntentId, OrderStatus status)
        {
            var order = await _orderRepo.GetOrderByPaymentIntentIdAsync(paymentIntentId);

            if (order == null)
            {
                _logger.LogWarning("Webhook received for unknown PaymentIntentId: {PaymentIntentId}", paymentIntentId);
                return;
            }

            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Complete)
            {
                _logger.LogInformation("Order {OrderId} is already {Status}. Ignoring payment status update.", order.Id, order.Status);
                return;
            }

            if (order.Status != status)
            {
                order.Status = status;
                await _orderRepo.UpdateAsync(order);
                _logger.LogInformation("Order {OrderId} status updated to {Status}", order.Id, status);
                await SendPaymentEmail(order, status);
            }
        }

        private async Task SendPaymentEmail(Order order, OrderStatus status)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(order.BuyerId)
                           ?? await _userManager.FindByIdAsync(order.BuyerId);

                var emailTo = user?.Email ?? order.BuyerId;

                if (string.IsNullOrEmpty(emailTo) || !emailTo.Contains("@"))
                {
                    _logger.LogWarning("Could not determine email for Order {OrderId}", order.Id);
                    return;
                }

                string subject;
                string body;

                if (status == OrderStatus.PaymentReceived)
                {
                    subject = $"Order Confirmed - #{order.Id}";
                    body = $"<h1>Thank you for your purchase!</h1><p>Your payment of ${order.GetTotal()} was successful.</p><p>We will ship your items soon.</p>";
                }
                else if (status == OrderStatus.PaymentFailed)
                {
                    subject = $"Payment Failed - Order #{order.Id}";
                    body = $"<h1>Payment Issue</h1><p>We could not process your payment for Order #{order.Id}. Please try again.</p>";
                }
                else
                {
                    return;
                }

                await _emailService.SendEmailAsync(emailTo, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email for order {OrderId}", order.Id);
            }
        }
    }
}