using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BccPay.Core.Infrastructure.PaymentModels.Request.Nets
{
    public class NetsPaymentRequest
    {
        [JsonPropertyName("order")]
        public Order Order { get; set; }

        [JsonPropertyName("checkout")]
        public CheckoutOnCreate Checkout { get; set; }

        [JsonPropertyName("notifications")]
        public Notifications Notifications { get; set; }

        public List<PaymentMethod> PaymentMethods { get; set; }
    }
}