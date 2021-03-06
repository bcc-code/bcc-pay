using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Enums;
using BccPay.Core.Notifications.Contracts;
using BccPay.Core.Notifications.Implementation;

namespace BccPay.Core.Domain.Entities;

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
    /// <summary>
    /// Optional property. 
    /// Could indicate if it is a membership deposit of saving payment
    /// </summary>
    public IPaymentDetails PaymentDetails { get; set; }
    public string PaymentType { get; set; }
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
        string description,
        IPaymentDetails paymentDetails,
        string paymentType)
    {
        PaymentId = Guid.NewGuid();
        PayerId = payerId;
        CurrencyCode = currencyCode;
        CountryCode = countryCode;
        Amount = amount;
        PaymentStatus = PaymentStatus.Pending;
        Created = DateTime.UtcNow;
        Description = description;
        PaymentDetails = paymentDetails;
        PaymentType = paymentType;
    }

    private void RefreshPaymentStatus()
    {
        Updated = DateTime.UtcNow;
        IsProblematic = Attempts.Where(x => x.AttemptStatus == AttemptStatus.PaidSucceeded).Count() > 1;

        PaymentStatus newStatus;

        if (Attempts.Any(x => x.AttemptStatus == AttemptStatus.PaidSucceeded))
            newStatus = PaymentStatus.Paid;
        else if (Attempts.Any(x => x.AttemptStatus == AttemptStatus.RefundedSucceeded))
            newStatus = PaymentStatus.Refunded;
        else if (Attempts.Where(x => x.AttemptStatus == AttemptStatus.Processing ||
                                x.AttemptStatus == AttemptStatus.WaitingForCharge ||
                                x.AttemptStatus == AttemptStatus.RefundedInitiated).Any())
            newStatus = PaymentStatus.Pending;
        else
            newStatus = PaymentStatus.Pending; // TODO: Set to close when ClosePayment(or etc.) endpoint appears

        if (newStatus != PaymentStatus)
        {
            var lastUpdatedAttempt = Attempts.OrderByDescending(x => x.Updated).First();

            Notifications.Add(new PaymentStateChangedNotification
            {
                Version = Notifications.OfType<PaymentStateChangedNotification>()
                    .Select(x => x.Version)
                    .DefaultIfEmpty()
                    .Max() + 1,
                PaymentId = PaymentId,
                FromPaymentStatus = PaymentStatus,
                ToPaymentStatus = newStatus,
                Amount = Amount,
                ProviderAmount = lastUpdatedAttempt.StatusDetails?.ProviderAmount,
                Currency = CurrencyCode,
                ProviderCurrency = lastUpdatedAttempt.StatusDetails?.ProviderCurrency,
                PaymentDetails = PaymentDetails,
                SuccessfulProviderDefinitionId = Attempts.Where(x
                    => x.AttemptStatus == AttemptStatus.PaidSucceeded)
                    .Select(x => x.ProviderDefinitionId)
                    .FirstOrDefault(),
                ProviderDefinitionId = lastUpdatedAttempt.ProviderDefinitionId,
                PaymentType = PaymentType,
            });

            PaymentStatus = newStatus;
        }
    }

    public void AddAttempt(
        IEnumerable<Attempt> attempts)
    {
        if (Attempts?.Any() == true)
        {
            Attempts.AddRange(attempts);
        }
        else
        {
            Attempts = new List<Attempt>(attempts);
        }
    }

    public void UpdateAttempt(Attempt attempt)
    {
        var attemptToUpdate = Attempts.Find(x => x.PaymentAttemptId == attempt.PaymentAttemptId);
        attemptToUpdate = attempt;
        attemptToUpdate.Updated = DateTime.UtcNow;

        RefreshPaymentStatus();
    }
}
