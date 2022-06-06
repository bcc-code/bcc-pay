namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class Company
{
    public string MerchantReference { get; set; }
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public ContactDetails ContactDetails { get; set; }
}
