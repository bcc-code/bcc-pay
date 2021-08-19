using System;

namespace BccPay.Core.Sample.Contracts.Responses
{
    public class MolliePaymentAttemptResponse : IPaymentAttemptResponse
    {
        public string CheckoutUrl { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
