using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions;

public class NullConfigurationException : Exception
{
    public NullConfigurationException(string client, string properties, HttpStatusCode statusCode = HttpStatusCode.ServiceUnavailable)
        : base($"Missing configurations for {client}, properties: {properties}")
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; private set; }
}
