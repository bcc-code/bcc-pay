using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions
{
    public class NullConfigurationException : Exception
    {
        public NullConfigurationException(string message = "Configuration must be set upped", HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
