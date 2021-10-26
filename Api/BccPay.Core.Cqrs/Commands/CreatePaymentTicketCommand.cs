using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentTicketCommand : IRequest
    {
        public string PaymentDefinitionId { get; set; }
        public Currencies BaseCurrency { get; set; }
        public Currencies DestinationCurrency { get; set; }
        public string PayerId { get; set; }
        public decimal SourceCurrencyAmount { get; set; }
    }

    public class CreatePaymentTicketCommandHandler : IRequestHandler<CreatePaymentTicketCommand>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IMediator _mediator;
        
        public CreatePaymentTicketCommandHandler(IAsyncDocumentSession documentSession, IMediator mediator)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreatePaymentTicketCommand request, CancellationToken cancellationToken)
        {
            var definition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                PaymentProviderDefinition.GetDocumentId(request.PaymentDefinitionId), cancellationToken);
            
            
            
           // _documentSession.StoreAsync<PaymentTicket>(new PaymentTicket());
            return new Unit();
        }
    }
}