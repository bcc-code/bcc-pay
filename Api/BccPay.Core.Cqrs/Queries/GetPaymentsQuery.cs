using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetPaymentsQuery : IRequest<PaymentsResult>
    {
        public GetPaymentsQuery()
        {
        }
    }

    public class PaymentResult
    {
        public Guid PaymentId { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Amount { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int AmountOfAttempts { get; set; }
    }

    public record PaymentsResult(List<PaymentResult> Payments);

    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, PaymentsResult>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentsQueryHandler(
            IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<PaymentsResult> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
        {
            var result = await (from payment in _documentSession.Query<Payment>()
                                select new PaymentResult
                                {
                                    PaymentId = payment.PaymentId,
                                    PayerId = payment.PayerId,
                                    CurrencyCode = payment.CurrencyCode,
                                    Amount = payment.Amount,
                                    CountryCode = payment.CountryCode,
                                    PaymentStatus = payment.PaymentStatus,
                                    Created = payment.Created,
                                    Updated = payment.Updated,
                                    AmountOfAttempts = payment.Attempts == null ? 0 : payment.Attempts.Count
                                }).ToListAsync(token: cancellationToken);

            return new PaymentsResult(result);
        }
    }
}
