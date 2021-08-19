using System;
using BccPay.Core.Contracts.Notifications;

namespace BccPay.Core.Implementation.Notifications
{
    public record PaymentCompletedNotification(Guid PaymentId) : IBccPayNotification
    {
        public DateTime When { get; } = DateTime.UtcNow;
    }
}

