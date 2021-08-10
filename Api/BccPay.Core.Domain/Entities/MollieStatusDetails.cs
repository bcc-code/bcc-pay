using System;

namespace BccPay.Core.Domain.Entities
{
    public class MollieStatusDetails : IStatusDetails
    {
        public string PaymentId { get; set; }
        public string CheckoutUrl { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Description { get; set; }
        public string WebhookUrl { get; set; }
        public bool IsSuccessful { get; set; }
        public CurrencyConversionRecord CurrencyConversionResult { get; set; }
    }
}
