using System;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Infrastructure.Configuration;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.Helpers.Implementation;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations.Mollie;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations.Nets;
using BccPay.Core.Infrastructure.RefitClients;
using BccPay.Core.Shared.Helpers;
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
            Action<InfrastructureConfigurations> configuration)
        {
            var defaultOptions = new InfrastructureConfigurations();
            configuration(defaultOptions);
            CheckSettings(defaultOptions);

            services.AddTransient(_ => defaultOptions.Settings);

            services.AddScoped<IPaymentProvider, NetsPaymentProvider>(implementationFactory => new NetsPaymentProvider(
                implementationFactory.GetRequiredService<INetsClient>(),
                defaultOptions.Nets,
                implementationFactory.GetRequiredService<IHttpContextAccessor>()));

            services.AddScoped<IPaymentProvider, MolliePaymentProvider>(implementationFactory =>
                new MolliePaymentProvider(defaultOptions.Mollie,
                    implementationFactory.GetRequiredService<IMollieClient>(),
                    implementationFactory.GetRequiredService<ICurrencyService>()));

            services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();
            services.AddScoped<IPaymentConfigurationsService, PaymentConfigurationsService>();

            services.AddRefitClient<INetsClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(defaultOptions.Nets.BaseAddress));
            services.AddRefitClient<IMollieClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(defaultOptions.Mollie.BaseAddress);
                    client.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.Authorization,
                        $"Bearer {defaultOptions.Mollie.AuthToken}");
                });
            services.AddRefitClient<IFixerClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(defaultOptions.Fixer.BaseAddress));

            services.AddTransient<ICurrencyService, CurrencyService>();
            return services;
        }

        public static void InitPaymentsConfiguration(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var paymentConfigurationsService =
                serviceScope.ServiceProvider.GetRequiredService<IPaymentConfigurationsService>();

            paymentConfigurationsService.InitPaymentsConfiguration();
        }

        private static void CheckSettings(InfrastructureConfigurations infrastructureConfigurations)
        {
            // NOTE: we have a lot of dependencies and validation is required for all of the payment providers
            if (infrastructureConfigurations.Mollie.IsAnyFieldNullOrEmpty(out string mollieFields))
                throw new NullConfigurationException(nameof(infrastructureConfigurations.Mollie), mollieFields);

            if (infrastructureConfigurations.Nets.IsAnyFieldNullOrEmpty(out string netsFields))
                throw new NullConfigurationException(nameof(infrastructureConfigurations.Nets), netsFields);

            if (infrastructureConfigurations.Fixer.IsAnyFieldNullOrEmpty(out string fixerFields))
                throw new NullConfigurationException(nameof(infrastructureConfigurations.Fixer), fixerFields);
        }
    }
}