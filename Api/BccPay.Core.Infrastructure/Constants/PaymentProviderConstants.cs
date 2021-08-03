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

            public static class WebhookEvents
            {
                public const string PaymentCreated = "payment.created";
                public const string CheckoutCompleted = "payment.checkout.completed";
                public const string ChargeCreated = "payment.charge.created.v2";
                public const string ChargeFailed = "payment.charge.failed";
            }
        }
    }
}
