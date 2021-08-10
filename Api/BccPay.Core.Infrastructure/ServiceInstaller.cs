using System;
using BccPay.Core.Infrastructure;
using BccPay.Core.Infrastructure.Configuration;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations.Mollie;
using BccPay.Core.Infrastructure.RefitClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
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
                return new NetsPaymentProvider(implementationFactory.GetRequiredService<INetsClient>(),
                    defaultOptions.Nets,
                    implementationFactory.GetRequiredService<IHttpContextAccessor>());
            });

            services.AddScoped<IPaymentProvider, MolliePaymentProvider>(implementationFactory =>
            {
                return new MolliePaymentProvider(defaultOptions.Mollie,
                    implementationFactory.GetRequiredService<IMollieClient>(),
                    implementationFactory.GetRequiredService<ICurrencyService>());
            });

            services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();
            services.AddScoped<IPaymentConfigurationsService, PaymentConfigurationsService>();

            services.AddRefitClient<INetsClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(defaultOptions.Nets.BaseAddress));
            services.AddRefitClient<IMollieClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(defaultOptions.Mollie.BaseAddress);
                    client.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.Authorization, $"Bearer {defaultOptions.Mollie.AuthToken}");
                });
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
