using BccPay.Core.Contracts.Responses;
using BccPay.Core.Domain.Entities;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Queries
{
    public record PaymentsResult(List<PaymentResponse> payments);

    public class GetPaymentsQuery : IRequest<PaymentsResult>
    {
    }

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
            try
            {
                var result = await (from payment in _documentSession.Query<Payment>()
                                    select new PaymentResponse
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
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                throw;
            }
        }
    }
}
