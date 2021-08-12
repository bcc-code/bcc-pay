using System;

namespace BccPay.Core.Contracts.Responses
{
    public class MolliePaymentAttemptResponse : IPaymentAttemptResponse
    {
        public string CheckoutUrl { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
