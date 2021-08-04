using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentModels.Webhooks;
using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static BccPay.Core.Infrastructure.Constants.PaymentProviderConstants.Nets;

namespace BccPay.Core.Cqrs.Commands
{
    public class UpdateNetsPaymentStatusCommand : IRequest<bool>
    {
        public UpdateNetsPaymentStatusCommand(string accessToken, NetsWebhook webhook)
        {
            AccessToken = accessToken;
            Webhook = webhook;
        }

        public string AccessToken { get; set; }
        public NetsWebhook Webhook { get; set; }
    }

    public class UpdateNetsPaymentAttemptCommandHandler : IRequestHandler<UpdateNetsPaymentStatusCommand, bool>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public UpdateNetsPaymentAttemptCommandHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<bool> Handle(UpdateNetsPaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                    Payment.GetPaymentId(Guid.Parse(request.Webhook.Data.PaymentId)), cancellationToken)
                ?? throw new NotFoundException("Invalid payment ID");

            if (payment.PaymentStatus == PaymentStatus.Canceled || payment.PaymentStatus == PaymentStatus.Completed)
                throw new InvalidPaymentException("Payment is not valid.");

            var actualAttempt = payment.Attempts
                    .Where(x => x.IsActive && x.NotificationAccessToken.Contains(request.AccessToken))
                    .FirstOrDefault()
                ?? throw new UpdatePaymentAttemptForbiddenException("Token is invalid or attempt is inactive.");

            var (webhookEvent, webhookStatus) = Webhooks.Messages
                .Where(x => x.Key == request.Webhook.Event.ToLower())
                .FirstOrDefault();

            actualAttempt.AttemptStatus = webhookEvent switch
            {
                Webhooks.PaymentCreated => actualAttempt.AttemptStatus = AttemptStatus.ProcessingPayment,
                Webhooks.CheckoutCompleted => actualAttempt.AttemptStatus = AttemptStatus.WaitingForCharge,
                Webhooks.ChargeCreated => actualAttempt.AttemptStatus = AttemptStatus.PaymentIsSuccessful,
                Webhooks.ChargeFailed => actualAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled,
                _ => actualAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled,
            };

            var statusDetails = StatusDetailsDeserializer<NetsStatusDetails>.GetStatusDetailsType(actualAttempt.StatusDetails);
            statusDetails.WebhookStatus = webhookStatus;
            actualAttempt.StatusDetails = statusDetails;

            payment.UpdateAttempt(actualAttempt);
            await _documentSession.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
