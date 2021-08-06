using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions
{
    [Serializable]
    public abstract class BccPayCoreException : Exception
    {
        public BccPayCoreException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
