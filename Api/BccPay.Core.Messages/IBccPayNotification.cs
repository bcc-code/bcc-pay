using System;

namespace BccPay.Core.Messages
{
    public interface IBccPayNotification
    {
        DateTime When { get; }
    }
}
