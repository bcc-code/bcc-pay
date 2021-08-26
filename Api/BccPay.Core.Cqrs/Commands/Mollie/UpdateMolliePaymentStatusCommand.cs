using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using BccPay.Core.Infrastructure.PaymentModels.Webhooks;
using BccPay.Core.Infrastructure.PaymentProviders;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands.Mollie
{
    public class UpdateMolliePaymentStatusCommand : IRequest<bool>
    {
        public UpdateMolliePaymentStatusCommand(MollieWebhook webhook, Guid paymentId)
        {
            PaymentId = paymentId;
            Webhook = webhook;
        }

        public Guid PaymentId { get; set; }
        public MollieWebhook Webhook { get; set; }
    }

    public class UpdateMolliePaymentStatusCommandHandler : IRequestHandler<UpdateMolliePaymentStatusCommand, bool>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public UpdateMolliePaymentStatusCommandHandler(IAsyncDocumentSession documentSession,
            IPaymentProviderFactory paymentProviderFactory)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
            _paymentProviderFactory = paymentProviderFactory
                ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
        }

        public async Task<bool> Handle(UpdateMolliePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                    Payment.GetDocumentId(request.PaymentId), cancellationToken)
                ?? throw new NotFoundException($"Invalid payment ID {request.PaymentId}");

            var actualAttempt = payment.Attempts
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault()
                    ?? throw new UpdatePaymentAttemptForbiddenException("Attempt is inactive.");

            var provider = _paymentProviderFactory.GetPaymentProvider(PaymentProvider.Mollie);

            var paymentResponse = await provider.GetPayment(request.Webhook.Id);

            var mollieStatusDetails = (MollieStatusDetails)actualAttempt.StatusDetails;

            if (mollieStatusDetails.MolliePaymentId != request.Webhook.Id)
                throw new InvalidPaymentException("Invalid mollie payment id");

            var molliePaymentResponse = ReverseAbstraction<MollieGetPaymentResponse, IPaymentResponse>.GetImplementationFromAbstraction(paymentResponse);

            if (molliePaymentResponse is not null)
            {
                actualAttempt.AttemptStatus = molliePaymentResponse.Status switch
                {
                    PaymentProviderConstants.Mollie.Webhook.Paid => AttemptStatus.Successful,
                    PaymentProviderConstants.Mollie.Webhook.Canceled => AttemptStatus.RejectedEitherCancelled,
                    PaymentProviderConstants.Mollie.Webhook.Pending => AttemptStatus.WaitingForCharge,
                    PaymentProviderConstants.Mollie.Webhook.Open => AttemptStatus.Processing,
                    PaymentProviderConstants.Mollie.Webhook.Failed => AttemptStatus.RejectedEitherCancelled,
                    PaymentProviderConstants.Mollie.Webhook.Expired => AttemptStatus.Expired,
                    _ => AttemptStatus.Processing
                };
                mollieStatusDetails.WebhookStatus = PaymentProviderConstants.Mollie.Webhook.Messages[molliePaymentResponse.Status];

                if (mollieStatusDetails.Errors != null)
                {
                    mollieStatusDetails.Errors.Add(molliePaymentResponse.Error);
                }
                else
                {
                    mollieStatusDetails.Errors = new List<string> { molliePaymentResponse.Error };
                }

                // NOTE: mollie removed "refund" status and force to check other
                // properties like "amountRefunded" to know, that Refund is triggered
                if (IsAmountValueGreaterThanZero(molliePaymentResponse.AmountRefunded?.Value))
                {
                    actualAttempt.AttemptStatus = AttemptStatus.RefundedSucceeded;
                }
            }

            actualAttempt.StatusDetails = mollieStatusDetails;
            payment.UpdateAttempt(actualAttempt);
            await _documentSession.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static bool IsAmountValueGreaterThanZero(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            if (decimal.TryParse(value, out decimal result))
                return result > 0;
            return false;
        }
    }
}
