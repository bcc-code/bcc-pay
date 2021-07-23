using BccPay.Core.Infrastructure.PaymentProviders;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetPaymentQuery : IRequest<string>
    {
        public GetPaymentQuery(string paymentId)
        {
            PaymentId = paymentId;
        }

        public string PaymentId { get; set; }

        public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, string>
        {
            private readonly IPaymentProviderFactory _paymentProviderFactory;

            public GetPaymentQueryHandler(IPaymentProviderFactory paymentProviderFactory)
            {
                _paymentProviderFactory = paymentProviderFactory
                    ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
            }

            public async Task<string> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
            {
                // TODO: 
                // var provider = _paymentProviderFactory.GetPaymentProvider("CreditCard"); // MOCK
                //await provider.GetPayment(request.PaymentId);

                throw new System.NotImplementedException();
            }
        }
    }
}
