using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Sample.Contracts.Responses;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentTicketByPayerIdQuery(string PayerId) : IRequest<PaymentTicketResponse>;

    public class
        GetPaymentTicketByPayerIdQueryHandler : IRequestHandler<GetPaymentTicketByPayerIdQuery, PaymentTicketResponse>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentTicketByPayerIdQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<PaymentTicketResponse> Handle(GetPaymentTicketByPayerIdQuery request,
            CancellationToken cancellationToken)
        {
            var ticket = await _documentSession.Query<PaymentTicket>()
                .Where(x => x.PayerId == request.PayerId)
                .OrderByDescending(x => x.Updated)
                .FirstOrDefaultAsync(cancellationToken);

            if (ticket is not null && !((ticket.Updated ?? ticket.Created) < DateTime.UtcNow.AddHours(-1)))
                return new PaymentTicketResponse(ticket.TicketId, ticket.BaseCurrencyAmount, ticket.OtherCurrencyAmount,
                    ticket.ExchangeRate, ticket.Updated ?? ticket.Created, ticket.PaymentDefinitionId, ticket.CountryCode);
            
            throw new Exception("Ticket is not valid");
        }
    }
}