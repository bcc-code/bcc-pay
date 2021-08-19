using System;
using BccPay.Core.Notifications.Contracts;

namespace BccPay.Core.Notifications.Implementation
{
    public record PaymentInitiatedNotification(Guid PaymentId) : IBccPayNotification
    {
        public DateTime When { get; } = DateTime.UtcNow;
    }
}
