using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, HttpStatusCode statusCode = HttpStatusCode.NotFound)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; private set; }
}
