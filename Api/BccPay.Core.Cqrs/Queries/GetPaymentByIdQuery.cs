using BccPay.Core.Domain.Entities;
using MediatR;
using Newtonsoft.Json;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetPaymentByIdQuery : IRequest<Payment>
    {
        public GetPaymentByIdQuery(Guid paymentId)
        {
            PaymentId = paymentId;
        }

        public Guid PaymentId { get; set; }
    }

    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Payment>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentByIdQueryHandler(
            IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<Payment> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(Payment.GetPaymentId(request.PaymentId), cancellationToken)
                    ?? throw new Exception("Invalid payment ID");

            //payment.Attempts.ForEach(x =>
            //JsonConvert.DeserializeObject<object>(
            //    JsonConvert.SerializeObject(x.StatusDetails, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }),
            //    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

            return payment;
        }
    }
}
