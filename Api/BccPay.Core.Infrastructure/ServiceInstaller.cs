using System;
using BccPay.Core.Infrastructure;
using BccPay.Core.Infrastructure.Configuration;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations;
using BccPay.Core.Infrastructure.RefitClients;
using Microsoft.AspNetCore.Builder;
using Refit;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceInstaller
    {
        public static IServiceCollection ConfigureBccPayInfrastructureServices(
            this IServiceCollection services,
            Action<BccPaymentsSettings> configuration)
        {
            var defaultOptions = new BccPaymentsSettings();
            configuration(defaultOptions);

            services.AddScoped<IPaymentProvider, NetsPaymentProvider>(implementationFactory =>
            {
                return new NetsPaymentProvider(implementationFactory.GetRequiredService<INetsClient>(), defaultOptions.Nets);
            });

            services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();
            services.AddScoped<IPaymentConfigurationsService, PaymentConfigurationsService>();
            services.AddRefitClient<INetsClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(defaultOptions.Nets.BaseAddress));

            services.AddRefitClient<IFixerClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(defaultOptions.Fixer.BaseAddress));

            services.AddTransient<ICurrencyService, CurrencyService>();
            return services;
        }

        public static void InitPaymentsConfiguration(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var paymentConfigurationsService = serviceScope.ServiceProvider.GetRequiredService<IPaymentConfigurationsService>();

            paymentConfigurationsService.InitPaymentsConfiguration();
        }
    }
}
