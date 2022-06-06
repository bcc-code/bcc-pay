using System.Collections.Generic;

namespace BccPay.Core.Infrastructure.Constants;

public static class PaymentProviderConstants
{
    public static class Nets
    {
        internal static class IntegrationType
        {
            public const string EmbeddedCheckout = nameof(EmbeddedCheckout);
            public const string HostedPaymentPage = nameof(HostedPaymentPage);
        }

        internal static class Order
        {
            public const string ItemReference = "FUNDS_DONATION";
            public const string ItemUnit = "FUNDS";
        }

        public static class Webhooks
        {
            public const string PaymentCreated = "payment.created";
            public const string CheckoutCompleted = "payment.checkout.completed";
            public const string ChargeCreated = "payment.charge.created.v2";
            public const string ChargeFailed = "payment.charge.failed";
            public const string RefundInitiated = "payment.refund.initiated.v2";
            public const string RefundFailed = "payment.refund.failed";
            public const string RefundCompleted = "payment.refund.completed";
            public const string CancelCreated = "payment.cancel.created";
            public const string CancelFailed = "payment.cancel.failed";

            public static readonly IReadOnlyDictionary<string, string> Messages
                = new Dictionary<string, string>()
                {
                    {PaymentCreated, "A payment has been created."},
                    {CheckoutCompleted, "The customer has completed the checkout."},
                    {ChargeCreated, "The customer has successfully been charged, partially or fully."},
                    {ChargeFailed, "A charge attempt has failed."},
                    {RefundInitiated, "A refund has been initiated."},
                    {RefundFailed, "A refund attempt has failed."},
                    {RefundCompleted, "A refund has successfully been completed."},
                    {CancelCreated, "A reservation has been canceled."},
                    {CancelFailed, "A cancellation has failed."}
                };
        }
    }

    public static class Mollie
    {
        public static class Webhook
        {
            public const string Open = "open";
            public const string Paid = "paid";
            public const string Expired = "expired";
            public const string Failed = "failed";
            public const string Pending = "pending";
            public const string Canceled = "canceled";

            public static readonly IReadOnlyDictionary<string, string> Messages
                = new Dictionary<string, string>()
                {
                    {Open, "The payment has been created."},
                    {Paid, "Payment is successfully paid."},
                    {Expired, "The payment has expired, e.g. your customer has abandoned the payment."},
                    {Failed, "The payment has failed and cannot be completed with a different payment method."},
                    {Pending, "The actual payment process has been started, but it’s not complete yet."},
                    {Canceled, "Customer has canceled the payment."}
                };
        }
    }
}
