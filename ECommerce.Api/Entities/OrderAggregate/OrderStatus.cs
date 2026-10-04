namespace ECommerce.Api.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        Pending,
        PaymentReceived,
        PaymentFailed,
        Shipped,
        Complete
    }
}
