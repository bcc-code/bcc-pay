using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Enums;
using BccPay.Core.Notifications;

namespace BccPay.Core.Domain.Entities
{
    public class Payment : INotificationsStore
    {
        public static string GetDocumentId(Guid paymentId)
            => $"payments/{paymentId}";

        public string Id => GetDocumentId(PaymentId);

        public Guid PaymentId { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<Attempt> Attempts { get; set; }
        public List<IBccPayNotification> Notifications { get; } = new List<IBccPayNotification>();

        public void Create(
            string payerId,
            string currencyCode,
            string countryCode,
            decimal amount,
            string description)
        {
            PaymentId = Guid.NewGuid();
            PayerId = payerId;
            CurrencyCode = currencyCode;
            CountryCode = countryCode;
            Amount = amount;
            PaymentStatus = PaymentStatus.Open;
            Created = DateTime.UtcNow;
            Description = description;
        }

        public void UpdatePaymentStatus(PaymentStatus paymentProgress)
        {
            if (paymentProgress == PaymentStatus.Canceled || paymentProgress == PaymentStatus.Completed)
            {
                this.CancelLastAttempt();
            }

            PaymentStatus = paymentProgress;

            Updated = DateTime.UtcNow;

            if (PaymentStatus == PaymentStatus.Completed)
            {
                this.Notifications.Add(new PaymentCompletedNotification(this.PaymentId));
            }
        }

        public void AddAttempt(
            List<Attempt> attempts)
        {
            if (Attempts?.Any() == true)
                Attempts.AddRange(attempts);
            else
                Attempts = new List<Attempt>(attempts);
        }

        public void UpdateAttempt(Attempt attempt)
        {
            var attemptToUpdate = Attempts.Find(x => x.PaymentAttemptId == attempt.PaymentAttemptId);
            attemptToUpdate = attempt;
            var paymentStatus = PaymentStatus;

            if (attempt.AttemptStatus == AttemptStatus.RejectedEitherCancelled)
            {
                attemptToUpdate.IsActive = false;
                paymentStatus = PaymentStatus.Canceled;
            }
            if (attempt.AttemptStatus == AttemptStatus.Expired)
            {
                attemptToUpdate.IsActive = false;
            }
            if (attempt.AttemptStatus == AttemptStatus.PaymentIsSuccessful)
            {
                attemptToUpdate.IsActive = false;
                paymentStatus = PaymentStatus.Completed;
            }

            UpdatePaymentStatus(paymentStatus);
        }

        public void CancelLastAttempt()
        {
            var lastAttempt = Attempts?.LastOrDefault();
            if (lastAttempt != null) lastAttempt.IsActive = false;
        }
    }
}
