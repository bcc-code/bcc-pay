using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Dtos;
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
                    ticket.BaseCurrencyAmount, ticket.OtherCurrencyAmount,
                    ticket.ExchangeRate, 1 / ticket.ExchangeRate, ticket.Updated ?? ticket.Created,
                    ticket.PaymentDefinitionId,
                    ticket.CountryCode);

            throw new Exception("Ticket is not valid");
        }
    }
}