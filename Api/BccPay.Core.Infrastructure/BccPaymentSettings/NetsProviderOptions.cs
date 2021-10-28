namespace BccPay.Core.Infrastructure.BccPaymentSettings
{
    public class NetsProviderOptions
    {
        public string BaseAddress { get; set; }
        public string CheckoutPageUrl { get; set; }
        public string SecretKey { get; set; }
        public string TermsUrl { get; set; }
        public string NotificationUrl { get; set; }
        public string MobileReturnUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
}