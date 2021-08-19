using System;
using BccPay.Core.Contracts.Notifications;

namespace BccPay.Core.Notifications.Implementation
{
    public record PaymentInitiatedNotification(Guid PaymentId) : IBccPayNotification
    {
        public DateTime When { get; } = DateTime.UtcNow;
    }
}
