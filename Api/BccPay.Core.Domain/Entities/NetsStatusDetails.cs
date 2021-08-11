namespace BccPay.Core.Domain.Entities
{
    public class NetsStatusDetails : IStatusDetails
    {
        public string PaymentCheckoutId { get; set; }
        public string WebhookStatus { get; set; }
        public string Error { get; set; }
        public bool IsSuccess { get; set; }
    }
}
