using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Shared.Converters;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentTicketByIdQuery(Guid TicketId) : IRequest<PaymentTicketResponse>;


    public class GetPaymentTicketByIdQueryHandler : IRequestHandler<GetPaymentTicketByIdQuery, PaymentTicketResponse>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentTicketByIdQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<PaymentTicketResponse> Handle(GetPaymentTicketByIdQuery request,
            CancellationToken cancellationToken)
        {
            var ticket = await _documentSession.LoadAsync<PaymentTicket>(PaymentTicket.GetDocumentId(request.TicketId),
                cancellationToken);

            if (ticket is not null && !((ticket.Updated ?? ticket.Created) < DateTime.UtcNow.AddHours(-1)))
                return new PaymentTicketResponse(ticket.TicketId, ticket.BaseCurrency, ticket.OtherCurrency,
                    ticket.BaseCurrencyAmount.ToAmountOfDigitsAfterPoint(), ticket.OtherCurrencyAmount.ToAmountOfDigitsAfterPoint(),
                    ticket.ExchangeRate.ToAmountOfDigitsAfterPoint(6),
                    (1 / ticket.ExchangeRate).ToAmountOfDigitsAfterPoint(6), ticket.Updated ?? ticket.Created,
                    ticket.PaymentDefinitionId,
                    ticket.CountryCode,
                    ticket.PaymentMethodMinimumAmount.ToAmountOfDigitsAfterPoint(2), 
                    ticket.PaymentMethodMaximumAmount.ToAmountOfDigitsAfterPoint(2));

            throw new Exception("Ticket is not valid");
        }
    }
}