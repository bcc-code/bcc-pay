using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions
{
    public class CurrencyExchangeOperationException : Exception
    {
        public CurrencyExchangeOperationException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
