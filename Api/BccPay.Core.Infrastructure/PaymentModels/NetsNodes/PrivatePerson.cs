namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    public class PrivatePerson
    {
        public string MerchantReference { get; set; }
        public string DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
    }
}