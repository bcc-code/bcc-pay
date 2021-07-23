namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    public class Item
    {
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Required]
        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [Required]
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// The tax/VAT rate (in percentage times 100). For examlpe, the value 2500 corresponds to 25%. Defaults to 0 if not provided.
        /// </summary>
        [JsonPropertyName("taxRate")]
        public int TaxRate { get; set; }

        /// <summary>
        /// The tax/VAT amount (unitPrice * quantity * taxRate / 10000). Defaults to 0 if not provided. taxAmount should include the total tax amount for the entire order item
        /// </summary>
        [JsonPropertyName("taxAmount")]
        public decimal TaxAmount { get; set; }

        [Required]
        [JsonPropertyName("netTotalAmount")]
        public decimal NetTotalAmount { get; set; }

        [Required]
        [JsonPropertyName("grossTotalAmount")]
        public decimal GrossTotalAmount { get; set; }
    }
}