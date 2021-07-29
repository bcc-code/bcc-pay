using BccPay.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BccPay.Core.Domain.Entities
{
    public class Payment
    {
        public static string GetPaymentId(Guid paymentId)
            => $"payments/{paymentId}";

        public string Id => GetPaymentId(PaymentId);

        public Guid PaymentId { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Amount { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<Attempt> Attempts { get; set; }

        public void Create(
            string payerId,
            string currencyCode,
            string countryCode,
            decimal amount)
        {
            PaymentId = Guid.NewGuid();
            PayerId = payerId;
            CurrencyCode = currencyCode;
            CountryCode = countryCode;
            Amount = amount;
            PaymentStatus = PaymentStatus.Open;
            Created = DateTime.Now;
        }

        public void Update(string currency,
            decimal amount)
        {
            CurrencyCode = currency;
            Amount = amount;
            Updated = DateTime.Now;
            PaymentStatus = PaymentStatus.Open;
        }

        public void UpdatePaymentStatus(PaymentStatus paymentProgress)
        {
            if (paymentProgress == PaymentStatus.Canceled || paymentProgress == PaymentStatus.Completed)
                Attempts.LastOrDefault().IsActive = false;

            PaymentStatus = paymentProgress;
            Updated = DateTime.Now;
        }

        public void AddAttempt(
            List<Attempt> attempts)
        {
            if (Attempts?.Any() == true)
                Attempts.AddRange(attempts);
            else
                Attempts = new List<Attempt>(attempts);
        }

        public void CancelLastAttempt()
            => Attempts.LastOrDefault().IsActive = false;
    }
}
