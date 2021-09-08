using System;
using System.Text.Json;
using BccPay.Core.Domain.Entities;

namespace BccPay.Core.Cqrs.CsvExports
{
    internal class NormalizePayment
    {
        public Guid PaymentId { get; set; }
        public string PayerId { get; set; }
        public string PaymentDetails { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsProblematicPayment { get; set; }
        public NormalizeAttempt Attempt { get; set; }

        public NormalizePayment(Payment payment)
        {
            Normalize(payment);
        }

        public NormalizePayment(Payment payment, Attempt attempt = null)
        {
            Normalize(payment, attempt);
        }

        private void Normalize(Payment payment, Attempt attempt = null)
        {
            Amount = payment.Amount;
            PayerId = payment.PayerId;
            Created = payment.Created;
            CurrencyCode = payment.CurrencyCode;
            IsProblematicPayment = payment.IsProblematic;
            PaymentStatus = Enum.GetName(payment.PaymentStatus);
            PaymentId = payment.PaymentId;
            PaymentDetails = payment.PaymentDetails is not null ? JsonSerializer.Serialize((object)payment.PaymentDetails) : null;
            Updated = payment.Updated;
            Attempt = attempt is not null
                ? new NormalizeAttempt(attempt)
                : null;
        }
    }

    internal class NormalizeAttempt
    {
        public Guid AttemptId { get; set; }
        public bool IsSuccessfulAttempt { get; set; }
        public string AttemptStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string DetailsJson { get; set; }

        internal NormalizeAttempt(Attempt attempt)
        {
            Normalize(attempt);
        }

        private void Normalize(Attempt attempt)
        {
            AttemptId = attempt.PaymentAttemptId;
            AttemptStatus = Enum.GetName(attempt.AttemptStatus);
            IsSuccessfulAttempt = attempt.AttemptStatus == Enums.AttemptStatus.PaidSucceeded;
            PaymentMethod = Enum.GetName(attempt.PaymentMethod);
            DetailsJson = attempt.StatusDetails is not null ? JsonSerializer.Serialize((object)attempt.StatusDetails) : null;
        }
    }
}
