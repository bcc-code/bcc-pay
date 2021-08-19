using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Extensions;
using BccPay.Core.DataAccess.Indexes;
using BccPay.Core.Enums;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetPaymentsWithFiltersQuery : IRequest<PaymentsResults>
    {
        public GetPaymentsWithFiltersQuery(
            int page,
            int size,
            DateTime? from,
            DateTime? to,
            PaymentStatus? paymentStatus,
            bool? isProblematicPayment)
        {
            Page = page;
            Size = size;
            From = from;
            To = to;
            PaymentStatus = paymentStatus;
            IsProblematicPayment = isProblematicPayment;
        }

        public int Page { get; set; }
        public int Size { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public bool? IsProblematicPayment { get; set; }
    }

    public record PaymentsResults(List<PaymentsIndex.Result> Payments);

    public class GetPaymentsWithFiltersQueryHandler : IRequestHandler<GetPaymentsWithFiltersQuery, PaymentsResults>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentsWithFiltersQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<PaymentsResults> Handle(GetPaymentsWithFiltersQuery request, CancellationToken cancellationToken)
        {
            var query = _documentSession.Query<PaymentsIndex.Result, PaymentsIndex>();

            if (request.IsProblematicPayment is not null)
                query = query.Where(x => x.IsProblematicPayment);

            if (request.PaymentStatus is not null)
                query = query.Where(x => x.PaymentStatus == request.PaymentStatus);

            if (request.From is not null && request.To is not null)
                query = query.Where(x => x.Created >= request.From && x.Created <= request.To);

            var result = await query.WithPagination(request.Page, request.Size)
                .ToListAsync(token: cancellationToken);

            return new PaymentsResults(result);
        }
    }
}
