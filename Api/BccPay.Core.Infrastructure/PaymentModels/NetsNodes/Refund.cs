namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.Collections.Generic;
    public class Refund
    {
        public string RefundId { get; set; }
        public int Amount { get; set; }
        public string State { get; set; }
        public string LastUpdated { get; set; }
        public List<Item> OrderItems { get; set; }
    }
}