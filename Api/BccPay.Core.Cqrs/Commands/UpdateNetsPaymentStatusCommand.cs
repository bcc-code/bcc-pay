using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.PaymentModels.Webhooks;
using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                ?? throw new Exception("Invalid payment id");

            if (payment.PaymentStatus == PaymentStatus.Canceled || payment.PaymentStatus == PaymentStatus.Completed)
                throw new Exception("Payment is not valid.");

            var actualAttempt = payment.Attempts
                    .Where(x => x.IsActive && x.NotificationAccessToken.Contains(request.AccessToken))
                    .FirstOrDefault()
                ?? throw new Exception("Invalid request: token is invalid or attempt is inactive.");

            actualAttempt.AttemptStatus = request.Webhook.Event.ToLower() switch
            {
                PaymentProviderConstants.Nets.WebhookEvents.PaymentCreated => actualAttempt.AttemptStatus = AttemptStatus.ProcessingPayment,
                PaymentProviderConstants.Nets.WebhookEvents.CheckoutCompleted => actualAttempt.AttemptStatus = AttemptStatus.WaitingForCharge,
                PaymentProviderConstants.Nets.WebhookEvents.ChargeCreated => actualAttempt.AttemptStatus = AttemptStatus.PaymentIsSuccessful,
                PaymentProviderConstants.Nets.WebhookEvents.ChargeFailed => actualAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled,
                _ => actualAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled,
            };

            payment.UpdateAttempt(actualAttempt);

            await _documentSession.SaveChangesAsync(cancellationToken);

            return true; // Webhook expects status code 200 otherwise - retry.
        }
    }
}
