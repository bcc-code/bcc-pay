using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.CsvExports;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Linq;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentsBase64CsvQuery(
            DateTime? From,
            DateTime? To,
            PaymentStatus? PaymentStatus,
            bool? IsProblematicPayment,
            string PaymentType = null,
            string PayerId = null) : IRequest<byte[]>;

    public class GetPaymentsToCsvQueryHandler : IRequestHandler<GetPaymentsBase64CsvQuery, byte[]>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentsToCsvQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<byte[]> Handle(GetPaymentsBase64CsvQuery request, CancellationToken cancellationToken)
        {
            var query = _documentSession.Query<Payment>();

            if (request.IsProblematicPayment is not null)
                query = query.Where(paymentIndex => paymentIndex.IsProblematic == request.IsProblematicPayment);

            if (!string.IsNullOrWhiteSpace(request.PayerId))
                query = query.Where(paymentIndex => paymentIndex.PayerId == request.PayerId);

            if (!string.IsNullOrWhiteSpace(request.PaymentType))
                query = query.Where(paymentIndex => paymentIndex.PaymentType == request.PaymentType);

            if (request.PaymentStatus is not null)
                query = query.Where(paymentIndex => paymentIndex.PaymentStatus == request.PaymentStatus);

            if (request.From is not null && request.To is not null)
                query = query.Where(paymentIndex => paymentIndex.Created >= request.From && paymentIndex.Created <= request.To);

            var results = await _documentSession.Advanced.StreamAsync(query, cancellationToken);

            List<Payment> payments = new();

            while (await results.MoveNextAsync())
            {
                payments.Add(results.Current.Document);
            }

            var normalizedPayments = NormalizePayments(payments);

            var paymentsCsvByteArray = ExportPaymentsToCsvQuery.CreateCSVEncodedPaymentResults(normalizedPayments
                .OrderByDescending(payment
                    => payment.Updated ?? payment.Created)
                .Reverse()
                .ToList());

            return paymentsCsvByteArray;
        }

        private static IEnumerable<NormalizePayment> NormalizePayments(List<Payment> payments)
        {
            var normalized = new List<NormalizePayment>();

            foreach (Payment payment in payments)
            {
                if (payment.Attempts is not null)
                    normalized.AddRange(payment.Attempts.Select(attempt => new NormalizePayment(payment, attempt)));
                else
                    normalized.Add(new NormalizePayment(payment));
            }

            return normalized;
        }
    }
}