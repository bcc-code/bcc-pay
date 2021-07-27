namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    public class Consumer
    {
        public Address ShippingAddress { get; set; }
        public Company Company { get; set; }
        public PrivatePerson PrivatePerson { get; set; }
        public Address BillingAddress { get; set; }
    }
}