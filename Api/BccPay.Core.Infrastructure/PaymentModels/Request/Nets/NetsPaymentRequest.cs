using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
using System.Collections.Generic;

namespace BccPay.Core.Infrastructure.PaymentModels.Request.Nets
{
    public class NetsPaymentRequest
    {
        public Order Order { get; set; }

        public CheckoutOnCreate Checkout { get; set; }

        public Notifications Notifications { get; set; }

        public List<PaymentMethod> PaymentMethods { get; set; }
    }
}