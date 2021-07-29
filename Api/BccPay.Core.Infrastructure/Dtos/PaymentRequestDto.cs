﻿namespace BccPay.Core.Infrastructure.Dtos
{
    public class PaymentRequestDto
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public string Email { get; set; }
        public string PhoneNumberPrefix { get; set; }
        public string PhoneNumberBody { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressDto Address { get; set; }
    }
}
