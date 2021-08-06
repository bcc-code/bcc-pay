using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceInstaller
    {
        public static IServiceCollection AddRavenDatabaseDocumentStore(this IServiceCollection services)
        {
            services.AddRavenDbDocStore();
            services.AddRavenDbDocStore(options =>
            {
                if (!string.IsNullOrWhiteSpace(options.Settings.CertFilePath))
                    options.Certificate = new X509Certificate2(Convert.FromBase64String(options.Settings.CertFilePath), options.Settings.CertPassword);
            });
            services.AddRavenDbAsyncSession();
            services.AddRavenDbSession();

            return services;
        }

        public static IApplicationBuilder WarmUpIndexesInRavenDatabase(
            this IApplicationBuilder app, params Assembly[] assembliesWithIndexes)
        {
            var docStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();
            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), docStore);
            foreach (Assembly assembly in assembliesWithIndexes)
            {
                IndexCreation.CreateIndexes(assembly, docStore);
            }

            return app;
        }
    }
}
