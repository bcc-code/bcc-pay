using System.Collections.Generic;

namespace BccPay.Core.Infrastructure.Constants
{
    public static class PaymentProviderConstants
    {
        public static class Nets
        {
            internal static class Order
            {
                public const string IntegrationType = "EmbeddedCheckout";
                public const string ItemReference = "FUNDS_DONATION";
                public const string ItemUnit = "FUNDS";
            }

            public static class Webhooks
            {
                public const string PaymentCreated =    "payment.created";
                public const string CheckoutCompleted = "payment.checkout.completed";
                public const string ChargeCreated =     "payment.charge.created.v2";
                public const string ChargeFailed =      "payment.charge.failed";

                public static readonly IReadOnlyDictionary<string, string> Messages
                    = new Dictionary<string, string>()
                    {
                        { PaymentCreated,    "A payment has been created." },
                        { CheckoutCompleted, "The customer has completed the checkout." },
                        { ChargeCreated,     "The customer has successfully been charged, partially or fully." },
                        { ChargeFailed,      "A charge attempt has failed." }
                    };
            }
        }
    }
}
