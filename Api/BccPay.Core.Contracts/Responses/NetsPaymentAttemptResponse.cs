namespace BccPay.Core.Contracts.Responses
{
    public class NetsPaymentAttemptResponse: IPaymentAttemptResponse
    {
        public string PaymentCheckoutId { get; set; }
    }
}
