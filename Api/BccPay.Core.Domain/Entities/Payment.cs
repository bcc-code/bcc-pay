using BccPay.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public PaymentProgress PaymentProgress { get; set; }
        public List<Attempt> Attempts { get; set; }

        public void Create(Guid paymentId,
            string payerId,
            string currency,
            decimal amount)
        {
            PaymentId = paymentId;
            PayerId = payerId;
            CurrencyCode = currency;
            Amount = amount;
            Created = DateTime.Now;
        }

        public void Update(string currency,
            decimal amount)
        {
            CurrencyCode = currency;
            Amount = amount;
            Updated = DateTime.Now;
        }
        public void DeactivateLastAttempt()
        {
            Attempts.LastOrDefault().IsActive = false;
        }

        public void AddAttempt(Attempt attempt)
        {
            Updated = DateTime.Now;
            Attempts.Add(attempt);
        }
    }
}
