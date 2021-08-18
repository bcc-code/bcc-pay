using System;

namespace BccPay.Core.Messages
{
    public record PaymentCompletedNotification(Guid PaymentId) : IBccPayNotification
    {
        public DateTime When { get; } = DateTime.UtcNow;
    }
}

