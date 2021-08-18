using System.Collections.Generic;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

namespace BccPay.Core.Infrastructure.PaymentModels.Response.Nets
{
    public class NetsRetrievePaymentResponse
    {
        public string PaymentId { get; set; }
        public Summary Summary { get; set; }
        public Consumer Consumer { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public OrderDetails Order { get; set; }
        public CheckoutOnRetrieve Checkout { get; set; }
        public string Created { get; set; }
        public List<Refund> Refunds { get; set; }
        public List<Charge> Charges { get; set; }
        public string Terminated { get; set; }
        public Subscription Subscription { get; set; }
    }
}
