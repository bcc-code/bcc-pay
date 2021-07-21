using System;

namespace BccPay.Core.Domain.EntitiesTEST
{
    public class Payment
    {
        public static string GetPaymentId(Guid paymentId)
        {
            return "payments/" + paymentId;
        }

        public string Id => GetPaymentId(PaymentId);

        public Guid PaymentId { get; set; }
        public Guid PayerId { get; set; }
        public decimal Amount { get; set; }
        public string Country { get; set; }
        public string PaymentInformation { get; set; }
        public string Details { get; set; }
    }
}
