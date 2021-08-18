using System;
using System.Collections.Generic;

namespace BccPay.Core.Domain.Entities
{
    public class MollieStatusDetails : IStatusDetails
    {
        public string MolliePaymentId { get; set; }
        public string CheckoutUrl { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Description { get; set; }
        public string WebhookUrl { get; set; }
        public string WebhookStatus { get; set; }
        public CurrencyConversionRecord CurrencyConversionResult { get; set; }

        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
    }
}
