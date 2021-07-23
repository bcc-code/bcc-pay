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

        public string PaymentId { get; set; }

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
                // TODO: 
                // var provider = _paymentProviderFactory.GetPaymentProvider("CreditCard"); // MOCK
                // await provider.GetPayment(request.PaymentId);

                throw new System.NotImplementedException();
            }
        }
    }
}
