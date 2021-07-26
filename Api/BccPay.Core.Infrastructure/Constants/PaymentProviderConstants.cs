﻿namespace BccPay.Core.Infrastructure.Constants
{
    public static class PaymentProviderConstants
    {
        public const string AuthorizationHeader =   "Authorization";
        public const string ContentType =           "content-type";

        public static class Nets
        {
            public const string SecretKey =         nameof(SecretKey);
            public const string IntegrationType =   "EmbeddedCheckout";
            public const string CheckoutPageUrl =   "http://localhost:63342";
            public const string TermsUrl =          "https://localhost:5001/terms";
            public const string ItemReference =     "FUNDS_DONATION";
            public const string ItemUnit =          "FUNDS";
            public const string WebHookUrl =        nameof(WebHookUrl);
            public const string WebHookEventName =  "payment.charge.created.v2";
        }
    }
}
