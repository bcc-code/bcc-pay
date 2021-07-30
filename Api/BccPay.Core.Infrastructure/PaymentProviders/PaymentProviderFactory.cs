using System.Collections.Generic;
using System.Linq;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    internal class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IEnumerable<IPaymentProvider> _paymentProviders;

        public PaymentProviderFactory(IEnumerable<IPaymentProvider> paymentProviders)
        {
            _paymentProviders = paymentProviders;
        }

        public IPaymentProvider GetPaymentProvider(string paymentMethod)
        {
            return _paymentProviders.First(x => x.PaymentMethod == paymentMethod);
        }
    }
}
