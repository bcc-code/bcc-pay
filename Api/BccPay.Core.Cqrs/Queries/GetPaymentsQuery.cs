using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetPaymentsQuery : IRequest<PaymentsResult>
    {
    }

    public class PaymentResult
    {
        public Guid PaymentId { get; set; }
        public string PaymentIdForCheckoutForm { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string CountryCode { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public PaymentProgress PaymentProgress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public record PaymentsResult(List<PaymentResult> payments);

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
                                    PaymentIdForCheckoutForm = payment.Attempts.LastOrDefault().PaymentIdForCheckoutForm,
                                    PayerId = payment.PayerId,
                                    CurrencyCode = payment.CurrencyCode,
                                    Amount = payment.Amount,
                                    CountryCode = payment.Attempts.LastOrDefault().CountryCode,
                                    Created = payment.Created,
                                    Updated = payment.Updated,
                                    PaymentProgress = payment.PaymentProgress,
                                    PaymentMethod = payment.Attempts.LastOrDefault().PaymentMethod
                                }).ToListAsync(token: cancellationToken);

            return new PaymentsResult(result);
        }
    }
}
