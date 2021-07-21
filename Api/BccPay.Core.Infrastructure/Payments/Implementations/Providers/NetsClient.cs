using BccPay.Core.Infrastructure.Enumerations;
using BccPay.Core.Infrastructure.Payments.Implementations.Providers.Nets.RequestModels;
using BccPay.Core.Infrastructure.Payments.Implementations.Providers.Nets.ResponseModels;
using RestSharp;
using System.Text.Json;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Payments.Implementations.Providers
{
    public class NetsClient : IPaymentProvider
    {
        public PaymentProvider PaymentProvider => PaymentProvider.Nets;

        public Task HandlePayment()
        {
            throw new System.NotImplementedException();
        }

        public Task RetrievePayment(string paymentId)
        {
            throw new System.NotImplementedException();
        }
    }
}
