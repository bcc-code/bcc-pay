using System.Text.Json.Serialization;

namespace BccPay.Core.Sample.Contracts.Responses;

public interface IPaymentAttemptResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Error { get; set; }
}
