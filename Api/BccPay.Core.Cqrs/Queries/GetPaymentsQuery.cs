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
        public Guid PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public int Amount { get; set; }
        public string CountryCode { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
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
                                    PaymentIdForCheckoutForm = payment.PaymentIdForCheckoutForm,
                                    PayerId = payment.PayerId,
                                    CurrencyCode = payment.CurrencyCode,
                                    Amount = payment.Amount,
                                    CountryCode = payment.CountryCode,
                                    Created = payment.Created,
                                    Updated = payment.Updated,
                                    PaymentStatus = payment.PaymentStatus,
                                    PaymentMethod = payment.PaymentMethod
                                }).ToListAsync(token: cancellationToken);

            return new PaymentsResult(result);
        }
    }
}
