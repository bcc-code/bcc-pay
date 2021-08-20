using System;
using System.Threading.Tasks;
using BccPay.Core.Notifications.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;

namespace BccPay.Core.DataAccess
{
    public interface IDocumentStoreListener
    {
        void OnAfterRavenDbSaveChanges(object sender, AfterSaveChangesEventArgs e);
    }

    internal class DocumentStoreListener : IDocumentStoreListener
    {
        private readonly IServiceProvider _serviceProvider;

        public DocumentStoreListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OnAfterRavenDbSaveChanges(object sender, AfterSaveChangesEventArgs e)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var mediator = serviceScope.ServiceProvider.GetService<IMediator>();

            if (mediator == null) return;

            if (e.Entity is IBccPayNotificationsStore notifications)
            {
                notifications.Notifications?.ForEach(
                    notification => Task.Run(async () => await mediator.Publish(notification)).Wait());

                notifications.Notifications?.Clear();
            }
        }
    }
}
