namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    public class Item
    {
        public string Reference { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public string Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public int TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal NetTotalAmount { get; set; }

        public decimal GrossTotalAmount { get; set; }
    }
}