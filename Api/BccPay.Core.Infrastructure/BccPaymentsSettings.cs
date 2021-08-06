using BccPay.Core.Infrastructure.PaymentProviders;

namespace BccPay.Core.Infrastructure
{
    public class BccPaymentsSettings
    {
        public NetsProviderOptions Nets { get; } = new NetsProviderOptions();
    }
}
