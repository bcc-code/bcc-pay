using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BccPay.Core.Domain.Entities;

public interface IStatusDetails
{
    public bool IsSuccess { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> Errors { get; set; }

    public string ProviderCurrency { get; }

    public decimal? ProviderAmount { get; }
}
