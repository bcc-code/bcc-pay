using System;
using MediatR;

namespace BccPay.Core.Contracts.Notifications
{
    public interface IBccPayNotification : INotification
    {
        DateTime When { get; }
    }
}
