using System.Text.Json.Serialization;

namespace BccPay.Core.Sample.Contracts.Responses;

public class NetsPaymentAttemptResponse : IPaymentAttemptResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string PaymentCheckoutId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string HostedPaymentPageUrl { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Error { get; set; }
}
