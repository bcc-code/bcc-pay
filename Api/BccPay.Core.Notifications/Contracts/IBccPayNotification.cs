using System;
using MediatR;

namespace BccPay.Core.Notifications.Contracts
{
    public interface IBccPayNotification : INotification
    {
        DateTime When { get; }
        int Version { get; }
    }
}
