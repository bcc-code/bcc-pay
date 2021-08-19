using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.DataAccess.Indexes;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetProblematicPaymentsQuery : IRequest<ProblematicPaymentsResult>
    {
    }

    public record ProblematicPaymentsResult(List<ProblematicPaymentsIndex.Result> Payments);

    public class GetProblematicPaymentsQueryHandler : IRequestHandler<GetProblematicPaymentsQuery, ProblematicPaymentsResult>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetProblematicPaymentsQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<ProblematicPaymentsResult> Handle(GetProblematicPaymentsQuery request, CancellationToken cancellationToken)
        {
            var queryResult = await _documentSession
                .Query<ProblematicPaymentsIndex.Result, ProblematicPaymentsIndex>()
                .ToListAsync(token: cancellationToken);

            return new ProblematicPaymentsResult(queryResult);
        }
    }
}
