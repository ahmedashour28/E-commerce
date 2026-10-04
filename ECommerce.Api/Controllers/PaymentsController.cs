using ECommerce.Api.Entities.OrderAggregate;
using ECommerce.Api.Services.Interfaces;
using ECommerce.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly string _whSecret;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPaymentService paymentService,
            IOptions<StripeSettings> settings,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
            _whSecret = settings.Value.WebhookSecret;
        }

        [Authorize]
        [HttpPost("{orderId}")]
        public async Task<ActionResult<Order>> CreateOrUpdatePaymentIntent(Guid orderId)
        {
            try
            {
                var order = await _paymentService.CreateOrUpdatePaymentIntentAsync(orderId);
                return Ok(new { order.PaymentIntentId, order.ClientSecret, order.Subtotal });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payment intent for order {OrderId}", orderId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _whSecret
                );

                PaymentIntent intent;

                _logger.LogInformation("Stripe Webhook received: {EventType}", stripeEvent.Type);

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogInformation("Payment Succeeded for Intent: {IntentId}", intent.Id);
                        await _paymentService.UpdateOrderPaymentStatus(intent.Id, OrderStatus.PaymentReceived);
                        break;

                    case "payment_intent.payment_failed":
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogWarning("Payment Failed for Intent: {IntentId}", intent.Id);
                        await _paymentService.UpdateOrderPaymentStatus(intent.Id, OrderStatus.PaymentFailed);
                        break;

                    default:
                        _logger.LogInformation("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe Webhook Error");
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General Webhook Error");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}