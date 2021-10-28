namespace BccPay.Core.Sample.Contracts.Responses
{
    public class NetsPaymentAttemptResponse : IPaymentAttemptResponse
    {
        public string PaymentCheckoutId { get; set; }
        public string HostedPaymentPageUrl { get; set; }
    }
}