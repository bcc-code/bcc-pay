﻿using System;
using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions
{
    public class UpdatePaymentAttemptForbiddenException : Exception
    {
        public UpdatePaymentAttemptForbiddenException(string message, HttpStatusCode statusCode = HttpStatusCode.Forbidden)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}

