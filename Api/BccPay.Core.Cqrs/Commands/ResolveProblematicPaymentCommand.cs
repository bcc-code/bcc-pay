using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.DataAccess.Indexes;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Exceptions;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public class ResolveProblematicPaymentCommand : IRequest
    {
        public ResolveProblematicPaymentCommand(Guid paymentId)
        {
            PaymentId = paymentId;
        }

        public Guid PaymentId { get; set; }
    }

    public class ResolveProblematicPaymentCommandHandler : IRequestHandler<ResolveProblematicPaymentCommand>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public ResolveProblematicPaymentCommandHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<Unit> Handle(ResolveProblematicPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(Payment.GetDocumentId(request.PaymentId), cancellationToken);

            if (payment is null)
                throw new InvalidPaymentException();

            var isPaymentProblematic = await _documentSession.Query<PaymentsIndex.Result, PaymentsIndex>()
                .Where(x => x.PaymentId == request.PaymentId
                    && x.IsProblematicPayment)
                .AnyAsync(token: cancellationToken);

            payment.ResolveProblematicPayment();

            await _documentSession.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
