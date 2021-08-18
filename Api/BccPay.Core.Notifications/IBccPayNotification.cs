using System;
using MediatR;

namespace BccPay.Core.Messages
{
    public interface IBccPayNotification : INotification
    {
        DateTime When { get; }
    }
}
