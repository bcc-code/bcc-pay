using System.Net;
using System;

namespace BccPay.Core.Infrastructure.Exceptions
{
    public class InvalidPaymentException : Exception
    {
        public InvalidPaymentException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
