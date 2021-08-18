using System;
using MediatR;

namespace BccPay.Core.Notifications
{
    public interface IBccPayNotification : INotification
    {
        DateTime When { get; }
    }
}
