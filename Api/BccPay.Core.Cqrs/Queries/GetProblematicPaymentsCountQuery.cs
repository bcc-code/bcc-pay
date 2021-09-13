using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetProblematicPaymentsCountQuery : IRequest<ProblematicPaymentsCountResult>
    {
    }

    public record ProblematicPaymentsCountResult(int Count);

    public class GetProblematicPaymentsCountQueryHandler : IRequestHandler<GetProblematicPaymentsCountQuery, ProblematicPaymentsCountResult>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetProblematicPaymentsCountQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;

        }

        public async Task<ProblematicPaymentsCountResult> Handle(GetProblematicPaymentsCountQuery request, CancellationToken cancellationToken)
        {
            var problematicPaymentsCount = await _documentSession
                .Query<Payment>()
                .Where(payment => payment.IsProblematic)
                .CountAsync(token: cancellationToken);

            return new ProblematicPaymentsCountResult(problematicPaymentsCount);
        }
    }
}
