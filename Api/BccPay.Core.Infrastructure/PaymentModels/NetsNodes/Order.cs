namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

        public class Order
        {
            /// <summary>
            /// Must contain at least one order item.
            /// </summary>
            [JsonPropertyName("items")]
            public List<Item> Items { get; set; }

            /// <summary>
            /// The amount of the order must match the sum of the specified order items.
            /// </summary>
            [JsonPropertyName("amount")]
            public int Amount { get; set; }

            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            [JsonPropertyName("reference")]
            public string Reference { get; set; }
        }
}