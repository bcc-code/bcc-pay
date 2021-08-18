using System.Threading.Tasks;
using BccPay.Core.Notifications;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.DataAccess
{
    public interface IDocumentStoreListener
    {
        void OnAfterRavenDbSaveChanges(object sender, AfterSaveChangesEventArgs e);
    }

    internal class DocumentStoreListener : IDocumentStoreListener
    {
        private readonly IMediator _mediator;

        public DocumentStoreListener(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public void OnAfterRavenDbSaveChanges(object sender, AfterSaveChangesEventArgs e)
        {
            if (e.Entity is INotificationsStore notifications)
            {
                notifications.Notifications?.ForEach(
                    notification => Task.Run(async () => await _mediator.Publish(notification)).Wait());

                notifications.Notifications?.Clear();
            }
        }
    }
}
