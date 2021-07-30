namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetPaymentProvider(string paymentMethod); // NOTE: maybe enumeration instead of string?
    }
}
