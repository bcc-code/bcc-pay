namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public class NetsProviderOptions
    {
        public string BaseAddress { get; set; }
        public string CheckoutPageUrl { get; set; }
        public string SecretKey { get; set; }
        public string TermsUrl { get; set; }
        public string NotificationUrl { get; set; }
    }

    public class PaymentProviderOptions
    {
        public NetsProviderOptions Nets { get; } = new NetsProviderOptions();
    }
}
