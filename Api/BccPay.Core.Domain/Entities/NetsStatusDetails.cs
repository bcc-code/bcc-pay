using System.Collections.Generic;

namespace BccPay.Core.Domain.Entities
{
    public class NetsStatusDetails : IStatusDetails
    {
        public string PaymentCheckoutId { get; set; }
        public string WebhookStatus { get; set; }
        public string InvoiceId { get; set; }
        public List<string> Errors { get; set; }
        public bool IsSuccess { get; set; }
    }
}
