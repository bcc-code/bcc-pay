using System;

namespace BccPay.Core.Domain.Entities
{
    public class MollieStatusDetails : IStatusDetails
    {
        public string PaymentId { get; set; }
        public string ProfileId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Description { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
