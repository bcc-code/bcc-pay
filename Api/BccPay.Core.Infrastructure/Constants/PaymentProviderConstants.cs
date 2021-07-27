namespace BccPay.Core.Infrastructure.Constants
{
    internal static class PaymentProviderConstants
    {
        internal const string AuthorizationHeader = "Authorization";
        internal const string ContentType = "content-type";

        internal static class Nets
        {
            public const string IntegrationType = "EmbeddedCheckout";
            public const string ItemReference = "FUNDS_DONATION";
            public const string ItemUnit = "FUNDS";
            public const string WebHookEventName = "payment.charge.created.v2";
        }
    }
}
