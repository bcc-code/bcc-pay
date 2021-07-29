namespace BccPay.Core.Domain.Entities
{
    public class NetsStatusDetails : IStatusDetails
    {
        public string CheckoutId { get; set; }
        public string Description { get; set; }
    }
}
