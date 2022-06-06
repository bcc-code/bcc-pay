using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.PaymentProviders;

internal class PaymentProviderFactory : IPaymentProviderFactory
{
    private readonly IEnumerable<IPaymentProvider> _paymentProviders;

    public PaymentProviderFactory(IEnumerable<IPaymentProvider> paymentProviders)
    {
        _paymentProviders = paymentProviders;
    }

    public IPaymentProvider GetPaymentProvider(PaymentProvider paymentProvider)
    {
        return _paymentProviders.First(x => x.PaymentProvider == paymentProvider);
    }
}
