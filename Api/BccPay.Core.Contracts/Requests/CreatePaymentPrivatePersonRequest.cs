namespace BccPay.Core.Contracts.Requests
{
    public class CreatePaymentPrivatePersonRequest
    {
        public string Email { get; set; }
        public string PhoneNumberPrefix { get; set; }
        public string PhoneNumberBody { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}
