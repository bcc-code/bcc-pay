using BccPay.Core.Enums;
using System;

namespace BccPay.Core.Domain.Entities
{
    public class Payment
    {
        public static string GetPaymentId(Guid paymentId)
        {
            return "payments/" + paymentId.ToString();
        }

        public string Id => GetPaymentId(PaymentId);

        public Guid PaymentId { get; private set; }
        public string PaymentIdForCheckoutForm { get; set; }
        public Guid PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string CountryCode { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentDetails PaymentDetails { get; set; } // TODO:

        public void Create(string providerPaymentId,
            Guid payerId,
            string currency,
            decimal amount,
            string country,
            PaymentMethod paymentMethod)
        {
            PaymentId = Guid.NewGuid();
            PaymentIdForCheckoutForm = providerPaymentId;
            PayerId = payerId;
            CurrencyCode = currency;
            Amount = amount;
            CountryCode = country;
            PaymentMethod = paymentMethod;
            PaymentStatus = PaymentStatus.Draft;
            Created = DateTime.Now;
            Updated = DateTime.Now;
        }

        public void Update(string providerPaymentId,
            string currency,
            decimal amount,
            string country,
            PaymentMethod paymentMethod,
            PaymentStatus paymentStatus)
        {
            PaymentIdForCheckoutForm = providerPaymentId;
            CurrencyCode = currency;
            Amount = amount;
            CountryCode = country;
            PaymentMethod = paymentMethod;
            PaymentStatus = paymentStatus;
            Updated = DateTime.Now;
        }
    }
}
