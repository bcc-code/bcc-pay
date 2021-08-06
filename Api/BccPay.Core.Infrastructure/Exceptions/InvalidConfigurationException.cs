using System.Net;

namespace BccPay.Core.Infrastructure.Exceptions
{
    public class InvalidConfigurationException : BccPayCoreException
    {
        public InvalidConfigurationException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, statusCode)
        {
        }
    }
}
