using System.Collections.Generic;
using BccPay.Core.Messages;

namespace BccPay.Core.Notifications
{
    public interface INotificationsStore
    {
        public List<IBccPayNotification> Notifications { get; }
    }
}
