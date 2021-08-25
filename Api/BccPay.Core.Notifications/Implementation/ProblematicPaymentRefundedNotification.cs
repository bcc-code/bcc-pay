using System;
using BccPay.Core.Notifications.Contracts;

namespace BccPay.Core.Notifications.Implementation
{
    public record ProblematicPaymentRefundedNotification(Guid PaymentId) : IBccPayNotification
    {
        public DateTime When { get; } = DateTime.UtcNow;
    }
}
