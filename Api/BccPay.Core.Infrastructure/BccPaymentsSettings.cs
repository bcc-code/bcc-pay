using BccPay.Core.Infrastructure.PaymentProviders;

namespace BccPay.Core.Infrastructure
{
    public class BccPaymentsSettings
    {
        public NetsProviderOptions Nets { get; } = new NetsProviderOptions();
        public FixerProviderOptions Fixer { get; } = new FixerProviderOptions();
    }
}
