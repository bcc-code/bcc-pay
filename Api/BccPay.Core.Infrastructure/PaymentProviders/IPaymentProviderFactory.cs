using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetPaymentProvider(PaymentProvider paymentProvider);
    }
}
