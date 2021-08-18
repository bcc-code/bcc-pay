using System.Collections.Generic;
using BccPay.Core.Messages;

namespace BccPay.Core.Domain
{
    public interface INotifications
    {
        public List<IBccPayNotification> Notifications { get; }
    }
}
