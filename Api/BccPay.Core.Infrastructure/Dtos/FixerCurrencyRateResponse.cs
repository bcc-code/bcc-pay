using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BccPay.Core.Infrastructure.Dtos;

public class FixerCurrencyRateResponse
{
    public bool Success { get; set; }
    public long Timestamp { get; set; }
    [JsonPropertyName("base")]
    public string BaseCurrency { get; set; }
    public string Date { get; set; }
    public IDictionary<string, decimal> Rates { get; set; }
}
