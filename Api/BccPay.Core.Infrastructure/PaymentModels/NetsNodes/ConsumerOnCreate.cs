namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class ConsumerOnCreate
{
    public string Reference { get; set; }
    public string Email { get; set; }
    public Address ShippingAddress { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public PrivatePersonOnCreate PrivatePerson { get; set; }
}
