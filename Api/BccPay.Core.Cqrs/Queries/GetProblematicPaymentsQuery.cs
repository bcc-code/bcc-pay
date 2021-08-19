using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Extensions;
using BccPay.Core.DataAccess.Indexes;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetProblematicPaymentsQuery : IRequest<ProblematicPaymentsResult>
    {
        public GetProblematicPaymentsQuery(int page, int size)
        {
            Page = page;
            Size = size;
        }

        public int Page { get; set; }
        public int Size { get; set; }
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
                .WithPagination(request.Page, request.Size)
                .ToListAsync(token: cancellationToken);

            return new ProblematicPaymentsResult(queryResult);
        }
    }
}
