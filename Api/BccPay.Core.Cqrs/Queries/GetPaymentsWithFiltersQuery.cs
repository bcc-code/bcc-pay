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
    public class GetPaymentsWithFiltersQuery : IRequest<PaymentWithFiltersResult>
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

    public class PaymentWithFiltersResult
    {
        public List<PaymentResult> Payments { get; set; }
        public int AmountOfObjects { get; set; }
    }

    public class PaymentResult
    {
        public Guid PaymentId { get; set; }
        public string PayerId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }
        public string Amount { get; set; }
        public string PaymentMethods { get; set; }
        public object PaymentDetails { get; set; }
    }

    public class GetPaymentsWithFiltersQueryHandler : IRequestHandler<GetPaymentsWithFiltersQuery, PaymentWithFiltersResult>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentsWithFiltersQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<PaymentWithFiltersResult> Handle(GetPaymentsWithFiltersQuery request, CancellationToken cancellationToken)
        {
            var query = _documentSession.Query<PaymentsIndex.Result, PaymentsIndex>();

            if (request.IsProblematicPayment is not null)
                query = query.Where(paymentIndex => paymentIndex.IsProblematic == request.IsProblematicPayment);

            if (request.PaymentStatus is not null)
                query = query.Where(paymentIndex => paymentIndex.PaymentStatus == request.PaymentStatus);

            if (request.From is not null && request.To is not null)
                query = query.Where(paymentIndex => paymentIndex.Created >= request.From && paymentIndex.Created <= request.To);

            var result = await query.WithPagination(request.Page, request.Size)
                .Select(paymentIndex => new PaymentResult
                {
                    Amount = paymentIndex.Amount + paymentIndex.CurrencyCode,
                    CountryCode = string.IsNullOrWhiteSpace(paymentIndex.CountryCode) ? paymentIndex.Attempts.First().CountryCode : paymentIndex.CountryCode,
                    Created = paymentIndex.Created,
                    Description = paymentIndex.Description,
                    PayerId = paymentIndex.PayerId,
                    PaymentId = paymentIndex.PaymentId,
                    Updated = paymentIndex.Updated,
                    PaymentMethods = paymentIndex.Attempts.Select(x => x.PaymentMethod.ToString()).ToString(),
                    PaymentDetails = (object)paymentIndex.PaymentDetails
                }).ToListAsync(token: cancellationToken);

            var amountOfObjects = await query.Distinct().CountAsync(cancellationToken);

            return new PaymentWithFiltersResult
            {
                Payments = result,
                AmountOfObjects = amountOfObjects
            };
        }
    }
}
