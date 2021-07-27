namespace BccPay.Core.Domain.Entities
{
    public class PaymentDetails
    {
        public string Currency { get; set; }
        public int Amount { get; set; }
        public string Reference { get; set; }
    }
}
