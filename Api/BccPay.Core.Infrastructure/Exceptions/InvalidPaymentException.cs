using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions;

public class InvalidPaymentException : Exception
{
    public InvalidPaymentException(string message = "Invalid payment", HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; private set; }
}
