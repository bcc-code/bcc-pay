using Microsoft.Extensions.DependencyInjection;
using Raven.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Bcc.Pay.Core.Infrastructure.Extensions
{
    public static class RavenDbConfigurationServiceCollectionExtension
    {
        public static IServiceCollection AddRavenDatabaseStore(this IServiceCollection services)
        {
            ServiceCollectionExtensions.AddRavenDbDocStore(services);
            services.AddRavenDbDocStore(options =>
            {
                if (!string.IsNullOrWhiteSpace(options.Settings.CertFilePath))
                    options.Certificate = new X509Certificate2(Convert.FromBase64String(options.Settings.CertFilePath), options.Settings.CertPassword);
            });
            services.AddRavenDbAsyncSession();

            return services;
        }
    }
}
