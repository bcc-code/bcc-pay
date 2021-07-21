using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BccPay.Core.Infrastructure.Payments.Implementations.Providers.Nets.RequestModels
{
    public class CreatePaymentRequest
    {
        [JsonPropertyName("order")]
        public Order Order { get; set; }

        [JsonPropertyName("checkout")]
        public Checkout Checkout { get; set; }

        [JsonPropertyName("notifications")]
        public Notifications Notifications { get; set; }

        public List<PaymentMethod> PaymentMethods { get; set; }
    }


    public class Checkout
    {
        [JsonPropertyName("termsUrl")]
        public string TermsUrl { get; set; }

        [JsonPropertyName("publicDevice")]
        public bool PublicDevice { get; set; }

        [JsonPropertyName("charge")]
        public bool Charge { get; set; }

        [JsonPropertyName("integrationType")]
        public string IntegrationType { get; set; }

        [JsonPropertyName("merchantHandlesConsumerData")]
        public bool MerchantHandlesConsumerData { get; set; }

        [JsonPropertyName("consumerType")]
        public ConsumerType ConsumerType { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class ConsumerType
    {
        [JsonPropertyName("supportedTypes")]
        public List<string> SupportedTypes { get; set; }

        [JsonPropertyName("default")]
        public string Default { get; set; }
    }

    public class Fee
    {
        public string Reference { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public int TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossTotalAmount { get; set; }
        public decimal NetTotalAmount { get; set; }
    }

    public class Item
    {
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Required]
        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [Required]
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// The tax/VAT rate (in percentage times 100). For examlpe, the value 2500 corresponds to 25%. Defaults to 0 if not provided.
        /// </summary>
        [JsonPropertyName("taxRate")]
        public int TaxRate { get; set; }

        /// <summary>
        /// The tax/VAT amount (unitPrice * quantity * taxRate / 10000). Defaults to 0 if not provided. taxAmount should include the total tax amount for the entire order item
        /// </summary>
        [JsonPropertyName("taxAmount")]
        public decimal TaxAmount { get; set; }

        [Required]
        [JsonPropertyName("netTotalAmount")]
        public int NetTotalAmount { get; set; }

        [Required]
        [JsonPropertyName("grossTotalAmount")]
        public int GrossTotalAmount { get; set; }
    }

    public class Notifications
    {
        [JsonPropertyName("webhooks")]
        public List<Webhook> Webhooks { get; set; }
    }

    public class Order
    {
        /// <summary>
        /// Must contain at least one order item.
        /// </summary>
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }

        /// <summary>
        /// The amount of the order must match the sum of the specified order items.
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }
    }

    public class PaymentMethod
    {
        public string Name { get; set; }
        public Fee Fee { get; set; }
    }

    public class Webhook
    {
        [JsonPropertyName("eventName")]
        public string EventName { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("authorization")]
        public string Authorization { get; set; }
    }
}
