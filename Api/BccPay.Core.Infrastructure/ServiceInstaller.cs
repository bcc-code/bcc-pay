using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations;
using BccPay.Core.Infrastructure.Validation;
using MediatR;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceInstaller
    {
        public static IServiceCollection ConfigureBccPayInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IPaymentProvider, CreditCardPaymentProvider>();
            services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();

            return services;
        }

        public static IServiceCollection AddValidation(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }
    }
}
