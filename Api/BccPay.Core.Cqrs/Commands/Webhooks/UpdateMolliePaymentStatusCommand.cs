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

namespace BccPay.Core.Cqrs.Commands.Webhooks
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
                    Payment.GetPaymentId(Guid.Parse(request.PaymentId)), cancellationToken)
                ?? throw new NotFoundException("Invalid payment ID");

            if (payment.PaymentStatus == PaymentStatus.Canceled || payment.PaymentStatus == PaymentStatus.Completed)
                throw new InvalidPaymentException("Payment is not valid.");

            var actualAttempt = payment.Attempts
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault()
                    ?? throw new UpdatePaymentAttemptForbiddenException("Attempt is inactive.");

            var provider = _paymentProviderFactory.GetPaymentProvider(PaymentProvider.Mollie);

            var paymentResponse = await provider.GetPayment(request.Webhook.Id);

            var mollieStatusDetails = ReverseAbstraction<MollieStatusDetails, IStatusDetails>.GetImplementationFromAbstraction(actualAttempt.StatusDetails);

            if (mollieStatusDetails.MolliePaymentId != request.Webhook.Id)
                throw new InvalidPaymentException("Invalid mollie payment id");

            var molliePaymentResponse = ReverseAbstraction<MollieGetPaymentResponse, IPaymentResponse>.GetImplementationFromAbstraction(paymentResponse);

            if (molliePaymentResponse is not null)
            {
                actualAttempt.AttemptStatus = molliePaymentResponse.Status switch
                {
                    PaymentProviderConstants.Mollie.Webhook.Paid => AttemptStatus.PaymentIsSuccessful,
                    PaymentProviderConstants.Mollie.Webhook.Canceled => AttemptStatus.RejectedEitherCancelled,
                    PaymentProviderConstants.Mollie.Webhook.Pending => AttemptStatus.WaitingForCharge,
                    PaymentProviderConstants.Mollie.Webhook.Open => AttemptStatus.ProcessingPayment,
                    PaymentProviderConstants.Mollie.Webhook.Failed => AttemptStatus.RejectedEitherCancelled,
                    PaymentProviderConstants.Mollie.Webhook.Expired => AttemptStatus.Expired,
                    _ => AttemptStatus.ProcessingPayment
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
            }

            actualAttempt.StatusDetails = mollieStatusDetails;
            payment.UpdateAttempt(actualAttempt);
            await _documentSession.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
