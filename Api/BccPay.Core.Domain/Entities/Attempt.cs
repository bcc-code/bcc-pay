using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Domain.Entities
{
    public class Attempt
    {
        public Guid PaymentAttemptId { get; set; }
        public bool IsActive { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime Created { get; set; }
        public AttemptStatus PaymentStatus { get; set; }
        public IStatusDetails StatusDetails { get; set; }
    }
}
