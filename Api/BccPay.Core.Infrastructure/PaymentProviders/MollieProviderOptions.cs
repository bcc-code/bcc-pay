namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public class MollieProviderOptions
    {
        public string BaseAddress { get; set; }
        public string AuthToken { get; set; }
        public string RedirectUrl { get; set; }
        public string RedirectUrlMobileApp { get; set; }
        public string WebhookUrl { get; set; }
        public decimal RateMarkup { get; set; }
    }
}
