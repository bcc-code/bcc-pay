using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceInstaller
    {
        public static IServiceCollection ConfigureBccPayInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IPaymentProvider, CreditCardPaymentProvider>();
            services.AddSingleton<IPaymentProviderFactory, PaymentProviderFactory>();

            return services;
        }
    }
}
