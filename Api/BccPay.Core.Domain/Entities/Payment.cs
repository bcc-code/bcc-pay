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
        public string ProviderPaymentId { get; set; }
        public Guid PayerId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Country { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentDetails PaymentDetails { get; set; }

        public void Create(string providerPaymentId,
            Guid payerId,
            string currency,
            decimal amount,
            string country,
            PaymentMethod paymentMethod)
        {
            PaymentId = Guid.NewGuid();
            ProviderPaymentId = providerPaymentId;
            PayerId = payerId;
            Currency = currency;
            Amount = amount;
            Country = country;
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
            ProviderPaymentId = providerPaymentId;
            Currency = currency;
            Amount = amount;
            Country = country;
            PaymentMethod = paymentMethod;
            PaymentStatus = paymentStatus;
            Updated = DateTime.Now;
        }
    }
}
