namespace BccPay.Core.Infrastructure.PaymentModels.Response.Nets
{
    using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
    using System.Collections.Generic;
    public class RetrievePaymentResponse
    {
        public string PaymentId { get; set; }
        public Summary Summary { get; set; }
        public Consumer Consumer { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public OrderDetails MyProperty { get; set; }
        public CheckoutOnRetrieve Checkout { get; set; }
        public string Created { get; set; }
        public List<Refund> Refunds { get; set; }
        public List<Charge> Charges { get; set; }
        public string Terminated { get; set; }
        public Subscription Subscription { get; set; }
    }
}