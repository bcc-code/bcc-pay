using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetPaymentsWithFiltersQuery : IRequest<List<GetPaymentWithFiltersResponse>>
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

    public class GetPaymentWithFiltersResponse
    {
        public Guid PaymentId { get; set; }
        public string PayerId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }
        public string Amount { get; set; }
        public string PaymentMethods { get; set; }
    }

    public class GetPaymentsWithFiltersQueryHandler : IRequestHandler<GetPaymentsWithFiltersQuery, List<GetPaymentWithFiltersResponse>>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentsWithFiltersQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<List<GetPaymentWithFiltersResponse>> Handle(GetPaymentsWithFiltersQuery request, CancellationToken cancellationToken)
        {
            var query = _documentSession.Query<PaymentsIndex.Result, PaymentsIndex>();

            if (request.IsProblematicPayment is not null)
                query = query.Where(x => x.IsProblematicPayment);

            if (request.PaymentStatus is not null)
                query = query.Where(x => x.PaymentStatus == request.PaymentStatus);

            if (request.From is not null && request.To is not null)
                query = query.Where(x => x.Created >= request.From && x.Created <= request.To);

            var result = await query.WithPagination(request.Page, request.Size)
                .Select(x => new GetPaymentWithFiltersResponse
                {
                    Amount = x.Amount + x.CurrencyCode,
                    CountryCode = string.IsNullOrWhiteSpace(x.CountryCode) ? x.Attempts.First().CountryCode : x.CountryCode,
                    Created = x.Created,
                    Description = x.Description,
                    PayerId = x.PayerId,
                    PaymentId = x.PaymentId,
                    Updated = x.Updated,
                    PaymentMethods = x.Attempts.Select(x => x.PaymentMethod.ToString()).ToString()
                })
                .ToListAsync(token: cancellationToken);

            return result;
        }
    }
}
