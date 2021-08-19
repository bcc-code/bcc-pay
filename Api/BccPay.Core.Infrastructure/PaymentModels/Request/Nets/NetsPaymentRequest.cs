using System.Collections.Generic;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

namespace BccPay.Core.Infrastructure.PaymentModels.Request.Nets
{
    public class NetsPaymentRequest
    {
        public Order Order { get; set; }

        public CheckoutOnCreate Checkout { get; set; }

        public NetsNodes.Notifications Notifications { get; set; }

        public List<PaymentMethod> PaymentMethods { get; set; }
    }
}