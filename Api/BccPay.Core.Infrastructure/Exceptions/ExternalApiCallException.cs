using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions;

public class ExternalApiCallException : Exception
{
    public ExternalApiCallException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; private set; }
}
