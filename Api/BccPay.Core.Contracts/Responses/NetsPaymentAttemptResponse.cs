namespace BccPay.Core.Contracts.Responses
{
    public class NetsPaymentAttemptResponse: IPaymentResponse
    {
        public string PaymentCheckoutId { get; set; }
    }
}
