namespace BccPay.Core.Infrastructure.Constants
{
    internal static class PaymentProviderConstants
    {
        internal const string AuthorizationHeader = "Authorization";
        internal const string ContentType = "content-type";

        internal static class Nets
        {
            public const string SecretKey = nameof(SecretKey);
            public const string IntegrationType = "EmbeddedCheckout";
            public const string CheckoutPageUrl = "https://localhost:8100";
            public const string TermsUrl = "https://localhost:8100/terms";
            public const string ItemReference = "FUNDS_DONATION";
            public const string ItemUnit = "FUNDS";
            public const string WebHookUrl = nameof(WebHookUrl);
            public const string WebHookEventName = "payment.charge.created.v2";
        }
    }
}
