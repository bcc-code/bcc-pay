using System;
using System.Text.Json;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;

namespace BccPay.Core.Cqrs.CsvExports
{
    internal class NormalizePayment
    {
        public Guid PaymentId { get; set; }
        public string MembershipId { get; set; }
        public string PaymentType { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
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
            Created = payment.Created;
            CurrencyCode = payment.CurrencyCode;
            IsProblematicPayment = payment.IsProblematic;
            MembershipId = "";
            PaymentStatus = payment.PaymentStatus;
            PaymentId = payment.PaymentId;
            PaymentType = payment.Type;
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
        public AttemptStatus AttemptStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string DetailsJson { get; set; }

        internal NormalizeAttempt(Attempt attempt)
        {
            Normalize(attempt);
        }

        private void Normalize(Attempt attempt)
        {
            AttemptId = attempt.PaymentAttemptId;
            AttemptStatus = attempt.AttemptStatus;
            IsSuccessfulAttempt = attempt.AttemptStatus == AttemptStatus.PaidSucceeded;
            PaymentMethod = attempt.PaymentMethod;
            DetailsJson = attempt.StatusDetails is not null ? JsonSerializer.Serialize((object)attempt.StatusDetails) : null;
        }
    }
}
