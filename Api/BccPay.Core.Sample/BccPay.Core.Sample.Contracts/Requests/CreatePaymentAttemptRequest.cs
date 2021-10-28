namespace BccPay.Core.Sample.Contracts.Requests
{
    public class CreatePaymentAttemptRequest
    {
        public string ProviderDefinitionId { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public bool IsHostedCheckout { get; set; }
    }
}
