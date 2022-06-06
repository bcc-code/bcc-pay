using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Invalid token", HttpStatusCode statusCode = HttpStatusCode.Unauthorized)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; private set; }
}
