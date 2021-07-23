using System;

namespace BccPay.Core.Infrastructure.Dtos
{
    public class PaymentRequestDto
    {
        public string Currency { get; set; }
        public string Country { get; set; }
        public decimal Amount { get; set; }
    }
}
