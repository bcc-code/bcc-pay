namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetPaymentProvider(string paymentMethod);
    }
}
