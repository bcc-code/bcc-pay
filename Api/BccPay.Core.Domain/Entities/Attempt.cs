using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Domain.Entities
{
    public class Attempt
    {
        public Guid PaymentAttemptId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public string CountryCode { get; set; }
        public DateTime Created { get; set; }
        public AttemptStatus AttemptStatus { get; set; }
        public IStatusDetails StatusDetails { get; set; }
        public string NotificationAccessToken { get; set; }
    }
}
