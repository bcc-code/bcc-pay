namespace BccPay.Core.Contracts.Requests
{
    public class CreatePaymentAttemptRequest
    {
        public string PaymentConfigurationId { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}
