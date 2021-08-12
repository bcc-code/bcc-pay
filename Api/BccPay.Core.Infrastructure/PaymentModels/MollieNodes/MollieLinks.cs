namespace BccPay.Core.Infrastructure.PaymentModels.MollieNodes
{
    public class MollieLinks
    {
        public Self Self { get; set; }
        public MollieCheckout Checkout { get; set; }
        public Dashboard Dashboard { get; set; }
        public Documentation Documentation { get; set; }
    }
}
