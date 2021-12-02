namespace BccPay.Core.Infrastructure.BccPaymentSettings
{
    public class MollieProviderOptions
    {
        public string BaseAddress { get; set; }
        public string AuthToken { get; set; }
        public string RedirectUrl { get; set; }
        public string WebhookUrl { get; set; }
    }
}
