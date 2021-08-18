using System.Collections.Generic;

namespace BccPay.Core.Notifications
{
    public interface INotificationsStore
    {
        public List<IBccPayNotification> Notifications { get; }
    }
}
