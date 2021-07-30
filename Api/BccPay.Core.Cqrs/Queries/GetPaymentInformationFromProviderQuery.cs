using BccPay.Core.Infrastructure.PaymentProviders;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetPaymentInformationFromProviderQuery : IRequest<string>
    {
        public GetPaymentInformationFromProviderQuery(string paymentId)
        {
            PaymentId = paymentId;
        }

        public string PaymentId { get; }
    }

    public class GetPaymentInformationFromProviderQueryHandler : IRequestHandler<GetPaymentInformationFromProviderQuery, string>
    {
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public GetPaymentInformationFromProviderQueryHandler(IPaymentProviderFactory paymentProviderFactory)
        {
            _paymentProviderFactory = paymentProviderFactory
                ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
        }

        public async Task<string> Handle(GetPaymentInformationFromProviderQuery request, CancellationToken cancellationToken)
        {
            // NOTE: Probably it will works only after checkout, Because it contains information, mostly, from checkout;
            // TODO: 
            // var provider = _paymentProviderFactory.GetPaymentProvider("CreditCard"); // MOCK
            // await provider.GetPayment(request.PaymentId);

            await Task.Delay(100, cancellationToken);
            throw new NotImplementedException();
        }
    }
}
