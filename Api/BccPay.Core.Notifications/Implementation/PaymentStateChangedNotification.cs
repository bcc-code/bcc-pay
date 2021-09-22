using System;
using BccPay.Core.Enums;
using BccPay.Core.Notifications.Contracts;

namespace BccPay.Core.Notifications.Implementation
{
    public class PaymentStateChangedNotification : IBccPayNotification
    {
        public Guid PaymentId { get; set; }
        public int Version { get; set; }
        public DateTime When { get; } = DateTime.UtcNow;

        public PaymentStatus FromPaymentStatus { get; set; }
        public PaymentStatus ToPaymentStatus { get; set; }
        public string SuccessfulProviderDefinitionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public object PaymentDetails { get; set; }
        public string PaymentType { get; set; }
        public string ProviderDefinitionId { get; set; }
    }
}
