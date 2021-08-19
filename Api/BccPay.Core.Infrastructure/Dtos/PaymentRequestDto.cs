namespace BccPay.Core.Infrastructure.Dtos
{
    public class PaymentRequestDto
    {
        public string PaymentId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public string Email { get; set; }
        public string PhoneNumberPrefix { get; set; }
        public string PhoneNumberBody { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NotificationAccessToken { get; set; }
        public string Description { get; set; }
        public string AcceptLanguage { get; set; }
        public bool IsMobile { get; set; }
        public AddressDto Address { get; set; }
    }
}
