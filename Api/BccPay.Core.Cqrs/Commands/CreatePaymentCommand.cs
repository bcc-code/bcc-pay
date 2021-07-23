using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.PaymentProviders;
using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentCommand : IRequest<string>
    {
        public CreatePaymentCommand(Guid payerId, string currency, decimal amount, string country, PaymentMethod paymentMethod)
        {
            PayerId = payerId;
            Currency = currency;
            Amount = amount;
            Country = country;
            PaymentMethod = paymentMethod;
        }

        public Guid PayerId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Country { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, string>
        {
            private readonly IAsyncDocumentSession _documentSession;
            private readonly IPaymentProviderFactory _paymentProviderFactory;

            public CreatePaymentCommandHandler(IPaymentProviderFactory paymentProviderFactory,
                IAsyncDocumentSession documentSession)
            {
                _documentSession = documentSession
                    ?? throw new ArgumentNullException(nameof(documentSession));
                _paymentProviderFactory = paymentProviderFactory
                    ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
            }

            public async Task<string> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
            {
                var provider = _paymentProviderFactory.GetPaymentProvider(request.PaymentMethod.ToString());

                var paymentId = await provider.CreatePayment(new Infrastructure.Dtos.PaymentRequestDto
                {
                    Amount = request.Amount,
                    Country = request.Country,
                    Currency = request.Currency
                });

                var payment = new Payment();
                payment.Create(paymentId, request.PayerId, request.Currency, request.Amount, request.Country, request.PaymentMethod);
                await _documentSession.StoreAsync(payment, cancellationToken);
                await _documentSession.SaveChangesAsync(cancellationToken);

                return payment.ProviderPaymentId;
            }
        }
    }
}
