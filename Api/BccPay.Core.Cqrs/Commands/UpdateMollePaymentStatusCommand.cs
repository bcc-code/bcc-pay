using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentModels.Webhooks;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public class UpdateMolliePaymentStatusCommand : IRequest<bool>
    {
        public UpdateMolliePaymentStatusCommand(MollieWebhook webhook, string paymentId)
        {
            PaymentId = paymentId;
            Webhook = webhook;
        }

        public string PaymentId { get; set; }
        public MollieWebhook Webhook { get; set; }
    }

    public class UpdateMolliePaymentStatusCommandHandler : IRequestHandler<UpdateMolliePaymentStatusCommand, bool>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public UpdateMolliePaymentStatusCommandHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<bool> Handle(UpdateMolliePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                    Payment.GetPaymentId(Guid.Parse(request.PaymentId)), cancellationToken)
                ?? throw new NotFoundException("Invalid payment ID");

            if (payment.PaymentStatus == PaymentStatus.Canceled || payment.PaymentStatus == PaymentStatus.Completed)
                throw new InvalidPaymentException("Payment is not valid.");

            var actualAttempt = payment.Attempts
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault()
                    ?? throw new UpdatePaymentAttemptForbiddenException("Attempt is inactive.");

            var statusDetails = StatusDetailsDeserializer<MollieStatusDetails>.GetStatusDetailsType(actualAttempt.StatusDetails);
            actualAttempt.StatusDetails = statusDetails;

            payment.UpdateAttempt(actualAttempt);
            await _documentSession.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
