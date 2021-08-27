﻿using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Enums;
using BccPay.Core.Notifications.Contracts;
using BccPay.Core.Notifications.Implementation;

namespace BccPay.Core.Domain.Entities
{
    public class Payment : IBccPayNotificationsStore
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
        public bool IsProblematic { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public List<Attempt> Attempts { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

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
            PaymentStatus = PaymentStatus.Pending;
            Created = DateTime.UtcNow;
            Description = description;
        }

        public void RefreshPaymentStatus()
        {
            Updated = DateTime.UtcNow;
            IsProblematic = Attempts.Where(x => x.AttemptStatus == AttemptStatus.PaidSucceeded).Count() > 1;

            if (Attempts.Where(x => x.AttemptStatus == AttemptStatus.Processing ||
                                    x.AttemptStatus == AttemptStatus.WaitingForCharge ||
                                    x.AttemptStatus == AttemptStatus.RefundedInitiated).Any())
                PaymentStatus = PaymentStatus.Pending;

            if (Attempts.Any(x => x.AttemptStatus == AttemptStatus.PaidSucceeded))
                PaymentStatus = PaymentStatus.Paid;

            if (Attempts.Any(x => x.AttemptStatus == AttemptStatus.RefundedSucceeded))
                PaymentStatus = PaymentStatus.Refunded;

            if (PaymentStatus == PaymentStatus.Paid)
                Notifications.Add(new PaymentSuccessfullyPaidNotification(PaymentId));

            if(PaymentStatus == PaymentStatus.Refunded)
                Notifications.Add(new PaymentRefundedNotification(PaymentId));
        }

        public void AddAttempt(
            List<Attempt> attempts)
        {
            if (Attempts?.Any() == true)
            {
                Attempts.AddRange(attempts);
            }
            else
            {
                Attempts = new List<Attempt>(attempts);
                Notifications.Add(new PaymentInitiatedNotification(PaymentId));
            }
        }

        public void UpdateAttempt(Attempt attempt)
        {
            var attemptToUpdate = Attempts.Find(x => x.PaymentAttemptId == attempt.PaymentAttemptId);
            attemptToUpdate = attempt;

            RefreshPaymentStatus();
        }
    }
}
