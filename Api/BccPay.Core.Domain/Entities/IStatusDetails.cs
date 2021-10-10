using System.Collections.Generic;
using System.Text.Json.Serialization;
using BccPay.Core.Enums;

namespace BccPay.Core.Domain.Entities
{
    public interface IStatusDetails
    {
        public bool IsSuccess { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors { get; set; }

        public Currencies Currency { get; }

        public decimal? AmountInCurrency { get; }
    }
}