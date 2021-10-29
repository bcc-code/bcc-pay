using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.CsvExports;
using BccPay.Core.Domain.Entities;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetPaymentsBase64CsvQuery : IRequest<byte[]>
    {
    }

    public class GetPaymentsToCsvQueryHandler : IRequestHandler<GetPaymentsBase64CsvQuery, byte[]>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentsToCsvQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<byte[]> Handle(GetPaymentsBase64CsvQuery request, CancellationToken cancellationToken)
        {
            var payments = await _documentSession.Query<Payment>()
                .ToListAsync(token: cancellationToken);

            var normalizedPayments = NormalizePayments(payments);

            var paymentsCsvByteArray = ExportPaymentsToCsvQuery.CreateCSVEncodedPaymentResults(normalizedPayments.OrderByDescending(payment
                => payment.Updated ?? payment.Created)
                    .Reverse()
                    .ToList());

            return paymentsCsvByteArray;
        }

        private static List<NormalizePayment> NormalizePayments(List<Payment> payments)
        {
            var normalized = new List<NormalizePayment>();

            foreach (Payment payment in payments)
            {
                if (payment.Attempts is not null)
                {
                    foreach (Attempt attempt in payment.Attempts)
                    {
                        normalized.Add(new NormalizePayment(payment, attempt));
                    }
                }
                else
                {
                    normalized.Add(new NormalizePayment(payment));
                }
            }

            return normalized;
        }
    }
}
