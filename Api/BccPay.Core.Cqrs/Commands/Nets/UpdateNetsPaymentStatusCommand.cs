using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.PaymentModels.Response.Nets;
using BccPay.Core.Infrastructure.PaymentModels.Webhooks;
using BccPay.Core.Infrastructure.PaymentProviders;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands.Nets
{
    public class UpdateNetsPaymentStatusCommand : IRequest<bool>
    {
        public UpdateNetsPaymentStatusCommand(Guid paymentId, string accessToken, NetsWebhook webhook)
        {
            AccessToken = accessToken;
            Webhook = webhook;
            PaymentId = paymentId;
        }

        public Guid PaymentId { get; set; }
        public string AccessToken { get; set; }
        public NetsWebhook Webhook { get; set; }
    }

    public class UpdateNetsPaymentAttemptCommandHandler : IRequestHandler<UpdateNetsPaymentStatusCommand, bool>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public UpdateNetsPaymentAttemptCommandHandler(IAsyncDocumentSession documentSession,
            IPaymentProviderFactory paymentProviderFactory)
        {
            _documentSession = documentSession;
            _paymentProviderFactory = paymentProviderFactory;
        }

        public async Task<bool> Handle(UpdateNetsPaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                    Payment.GetDocumentId(request.PaymentId), cancellationToken)
                ?? throw new NotFoundException($"Invalid payment ID {request.PaymentId}");

            var provider = _paymentProviderFactory.GetPaymentProvider(PaymentProvider.Nets);

            if (!payment.Attempts.Where(x => x.NotificationAccessToken == request.AccessToken).Any())
                throw new UnauthorizedException();

            var actualAttempt = payment.Attempts
                    .Where(x => x.NotificationAccessToken == request.AccessToken)
                    .FirstOrDefault()
                    ?? throw new UpdatePaymentAttemptForbiddenException("Invalid attempt token.");

            var (webhookEvent, webhookStatus) = PaymentProviderConstants.Nets.Webhooks.Messages
                .Where(x => x.Key == request.Webhook.Event.ToLower())
                .FirstOrDefault();

            actualAttempt.AttemptStatus = webhookEvent switch
            {
                PaymentProviderConstants.Nets.Webhooks.PaymentCreated => actualAttempt.AttemptStatus = AttemptStatus.Processing,
                PaymentProviderConstants.Nets.Webhooks.RefundInitiated => actualAttempt.AttemptStatus = AttemptStatus.RefundedInitiated,
                PaymentProviderConstants.Nets.Webhooks.CheckoutCompleted => actualAttempt.AttemptStatus = AttemptStatus.WaitingForCharge,
                PaymentProviderConstants.Nets.Webhooks.ChargeCreated => actualAttempt.AttemptStatus = AttemptStatus.PaidSucceeded,
                PaymentProviderConstants.Nets.Webhooks.ChargeFailed => actualAttempt.AttemptStatus = AttemptStatus.Failed,
                PaymentProviderConstants.Nets.Webhooks.RefundCompleted => actualAttempt.AttemptStatus = AttemptStatus.RefundedSucceeded,
                _ => AttemptStatus.Processing,
            };

            var statusDetails = (NetsStatusDetails)actualAttempt.StatusDetails;
            statusDetails.WebhookStatus = webhookStatus;

            if (webhookEvent == PaymentProviderConstants.Nets.Webhooks.CheckoutCompleted)
            {
                var providerPaymentInformation = (NetsGetPaymentResponse)await provider.GetPayment(request.Webhook.Data.PaymentId)
                    ?? throw new UnauthorizedException();

                if (!ValidateChargeAmount(request.Webhook, providerPaymentInformation))
                    throw new UpdatePaymentAttemptForbiddenException("Unable to charge payment, invalid reserved amount.");

                await provider.ChargePayment(payment, actualAttempt);
            }

            payment.UpdateAttempt(actualAttempt);

            await _documentSession.SaveChangesAsync(cancellationToken);

            return true;
        }

        // NOTE: Additional check before charging.
        private bool ValidateChargeAmount(NetsWebhook webhook, NetsGetPaymentResponse providerResponse)
            => webhook.Data.Order.Amount.TotalAmount == providerResponse.Summary.ReservedAmount;
    }
}
