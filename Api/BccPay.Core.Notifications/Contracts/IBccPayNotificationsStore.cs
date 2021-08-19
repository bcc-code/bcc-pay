using System.Collections.Generic;

namespace BccPay.Core.Contracts.Notifications
{
    public interface IBccPayNotificationsStore
    {
        public List<IBccPayNotification> Notifications { get; }
    }
}
