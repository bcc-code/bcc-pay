using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentModels.Webhooks;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands.Webhooks
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

            //if (payment.PaymentStatus == PaymentStatus.Canceled || payment.PaymentStatus == PaymentStatus.Completed)
            //    throw new InvalidPaymentException("Payment is not valid.");

            if (!payment.Attempts.Where(x => x.NotificationAccessToken.Contains(request.AccessToken)).Any())
                throw new UnauthorizedException();

            var actualAttempt = payment.Attempts
                    .Where(x => x.IsActive && x.NotificationAccessToken.Contains(request.AccessToken))
                    .FirstOrDefault()
                    ?? throw new UpdatePaymentAttemptForbiddenException("Attempt is inactive.");

            var (webhookEvent, webhookStatus) = PaymentProviderConstants.Nets.Webhooks.Messages
                .Where(x => x.Key == request.Webhook.Event.ToLower())
                .FirstOrDefault();

            actualAttempt.AttemptStatus = webhookEvent switch
            {
                PaymentProviderConstants.Nets.Webhooks.PaymentCreated => actualAttempt.AttemptStatus = AttemptStatus.ProcessingPayment,
                PaymentProviderConstants.Nets.Webhooks.CheckoutCompleted => actualAttempt.AttemptStatus = AttemptStatus.WaitingForCharge,
                PaymentProviderConstants.Nets.Webhooks.ChargeCreated => actualAttempt.AttemptStatus = AttemptStatus.PaymentIsSuccessful,
                PaymentProviderConstants.Nets.Webhooks.ChargeFailed => actualAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled,
                _ => actualAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled,
            };

            var statusDetails = ReverseAbstraction<NetsStatusDetails, IStatusDetails>.GetImplementationFromAbstraction(actualAttempt.StatusDetails);
            statusDetails.WebhookStatus = webhookStatus;
            actualAttempt.StatusDetails = statusDetails;

            payment.UpdateAttempt(actualAttempt);
            await _documentSession.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
