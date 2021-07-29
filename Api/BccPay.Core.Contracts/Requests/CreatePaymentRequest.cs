namespace BccPay.Core.Contracts.Requests
{
    public class CreatePaymentRequest
    {
        public string PayerId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
