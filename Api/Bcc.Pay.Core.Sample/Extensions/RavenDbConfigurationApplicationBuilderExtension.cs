using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Bcc.Pay.Core.Sample.Extensions
{
    public static class RavenDbConfigurationApplicationBuilderExtension
    {
        public static IApplicationBuilder WarmUpRavenDatabase(this IApplicationBuilder app)
        {
            var docStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();
            // IndexCreation.CreateIndexes(typeof(/*IndexName*/).Assembly, docStore); // Indexes location

            try
            {
                using var dbSession = docStore.OpenSession();
                //_ = dbSession.Query</*EntityName*/>().Take(0).ToList(); // Entities location                
            }
            catch (Raven.Client.Exceptions.Database.DatabaseDoesNotExistException)
            {
                docStore.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord
                {
                    DatabaseName = docStore.Database
                }));
            }

            return app;
        }
    }
}
