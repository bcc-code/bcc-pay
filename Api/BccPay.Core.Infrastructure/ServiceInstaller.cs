using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Infrastructure.PaymentProviders.Implementations;
using BccPay.Core.Infrastructure.PaymentProviders.RefitClients;
using Microsoft.AspNetCore.Builder;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.DependencyInjection;
using Refit;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceInstaller
    {
        public static IServiceCollection ConfigureBccPayInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IPaymentProvider, CreditCardPaymentProvider>();
            services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();


            services.AddRefitClient<INetsClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://test.api.dibspayment.eu"));

            return services;
        }

        #region RavenDB
        public static IServiceCollection AddRavenDatabaseDocumentStore(this IServiceCollection services)
        {

            Raven.DependencyInjection.ServiceCollectionExtensions.AddRavenDbDocStore(services);
            services.AddRavenDbDocStore(options =>
            {
                if (!string.IsNullOrWhiteSpace(options.Settings.CertFilePath))
                    options.Certificate = new X509Certificate2(Convert.FromBase64String(options.Settings.CertFilePath), options.Settings.CertPassword);
            });
            services.AddRavenDbAsyncSession();

            return services;
        }

        public static IApplicationBuilder WarmUpIndexesInRavenDatabase(this IApplicationBuilder app, Assembly assemblyWithIndexes)
        {
            var docStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();

            IndexCreation.CreateIndexes(assemblyWithIndexes, docStore);

            return app;
        }

        public static IApplicationBuilder WarmUpIndexesInRavenDatabase(this IApplicationBuilder app, Assembly[] assembliesWithIndexes)
        {
            var docStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();

            foreach (Assembly assembly in assembliesWithIndexes)
            {
                IndexCreation.CreateIndexes(assembly, docStore);
            }

            return app;
        }
        #endregion
    }
}
