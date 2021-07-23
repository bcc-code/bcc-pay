using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.DependencyInjection;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace BccPay.Core.Infrastructure.Extensions
{
    public static class RavenDatabaseExtensions
    {
        public static IServiceCollection AddRavenDatabaseDocumentStore(this IServiceCollection services)
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
    }
}
