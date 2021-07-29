using System;

namespace BccPay.Core.Contracts.Requests
{
    public class CreatePaymentAttemptRequest
    {
        public string PaymentMethod { get; set; }
        public Guid PaymentId { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string CountryCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}
