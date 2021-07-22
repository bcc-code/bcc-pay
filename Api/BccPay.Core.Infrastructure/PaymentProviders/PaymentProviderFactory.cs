using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    internal class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IEnumerable<IPaymentProvider> paymentProviders;

        public PaymentProviderFactory(IEnumerable<IPaymentProvider> paymentProviders)
        {
            this.paymentProviders = paymentProviders;
        }

        public IPaymentProvider GetPaymentProvider(string paymentMethod)
        {
            return this.paymentProviders.First(x => x.PaymentMethod == paymentMethod);
        }
    }
}
