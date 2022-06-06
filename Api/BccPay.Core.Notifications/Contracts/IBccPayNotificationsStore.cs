using System.Collections.Generic;

namespace BccPay.Core.Notifications.Contracts;

public interface IBccPayNotificationsStore
{
    public List<IBccPayNotification> Notifications { get; }
}
