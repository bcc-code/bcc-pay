using System;

namespace BccPay.Core.Notifications
{
    public record PaymentCompletedNotification(Guid PaymentId) : IBccPayNotification
    {
        public DateTime When { get; } = DateTime.UtcNow;
    }
}

