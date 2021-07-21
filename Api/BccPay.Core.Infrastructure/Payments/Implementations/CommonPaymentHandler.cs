using BccPay.Core.Infrastructure.Enumerations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Payments.Implementations
{
    public class CommonPaymentHandler : IPaymentHandler
    {
        private readonly IEnumerable<IPaymentProvider> _paymentProviders;

        public CommonPaymentHandler(IEnumerable<IPaymentProvider> paymentProviders)
        {
            _paymentProviders = paymentProviders;
        }

        public PaymentProvider PaymentProvider { get; }

        public Task ProcessPayment()
        {
            throw new System.NotImplementedException();
        }
    }
}
