using System;
using System.Text.Json.Serialization;

namespace BccPay.Core.Sample.Contracts.Responses
{
    public class MolliePaymentAttemptResponse : IPaymentAttemptResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string CheckoutUrl { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime ExpiresAt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Error { get; set; }
    }
}
