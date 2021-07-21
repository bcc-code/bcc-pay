using BccPay.Core.Infrastructure.Enumerations;
using System;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Payments.Implementations.Providers
{
    public class VippsClient : IPaymentProvider
    {
        public PaymentProvider PaymentProvider => PaymentProvider.Vipps;

        public Task HandlePayment()
        {
            throw new NotImplementedException();
        }

        public Task RetrievePayment(string paymentId)
        {
            throw new NotImplementedException();
        }
    }
}
