using BccPay.Core.Contracts.Requests.Nodes;

namespace BccPay.Core.Contracts.Requests
{
    /// <summary>
    /// Only required properties and webhooks
    /// </summary>
    public class CreatePaymentRequest
    {
        public OrderRequest Order { get; set; }
        public CheckoutRequest CheckoutRequest { get; set; }
        public NotificationRequest Notification { get; set; }
    }
}
