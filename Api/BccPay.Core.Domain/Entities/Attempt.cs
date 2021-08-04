using BccPay.Core.Enums;
using System;

namespace BccPay.Core.Domain.Entities
{
    public class Attempt
    {
        public Guid PaymentAttemptId { get; set; }
        public bool IsActive { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime Created { get; set; }
        public AttemptStatus AttemptStatus { get; set; }
        public IStatusDetails StatusDetails { get; set; }
        public string NotificationAccessToken { get; set; }
    }
}
