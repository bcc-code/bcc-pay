using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Sample.Contracts.Requests
{
    public class PaymentFilters : BasePaging
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public bool? IsProblematicPayment { get; set; }
        public string PaymentType { get; set; }
        public string PayerId { get; set; }
    }
}
