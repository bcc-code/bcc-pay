using System;
using BccPay.Core.Shared.Helpers;

namespace BccPay.Core.Infrastructure.BccPaymentSettings
{
    public class InfrastructureConfigurations
    {
        public void AddNets(Action<NetsProviderOptions> configure)
            => configure(Nets);

        public void AddMollie(Action<MollieProviderOptions> configure)
            => configure(Mollie);

        public void AddFixer(Action<FixerProviderOptions> configure)
            => configure(Fixer);

        public void AddCustomSettings(Action<InternalSettings> configure)
            => configure(Settings);

        internal NetsProviderOptions Nets { get; } = new NetsProviderOptions();
        internal FixerProviderOptions Fixer { get; } = new FixerProviderOptions();
        internal MollieProviderOptions Mollie { get; } = new MollieProviderOptions();
        internal InternalSettings Settings { get; } = new InternalSettings();
    }
}
