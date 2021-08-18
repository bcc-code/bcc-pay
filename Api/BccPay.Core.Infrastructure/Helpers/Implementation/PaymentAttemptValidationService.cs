using System;
using System.Linq;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using BccPay.Core.Infrastructure.PaymentProviders;
using Microsoft.Extensions.Logging;

namespace BccPay.Core.Infrastructure.Helpers.Implementation
{
    public class PaymentAttemptValidationService : IPaymentAttemptValidationService
    {
        private readonly IPaymentProviderFactory _paymentProviderFactory;
        private readonly ILogger<PaymentAttemptValidationService> _logger;

        public PaymentAttemptValidationService(IPaymentProviderFactory paymentProviderFactory,
            ILogger<PaymentAttemptValidationService> logger)
        {
            _paymentProviderFactory = paymentProviderFactory;
            _logger = logger;
        }

        public async Task<bool> TryCancelPreviousPaymentAttempt(Payment payment)
        {
            var lastAttempt = payment.Attempts.LastOrDefault();

            if (lastAttempt.StatusDetails.GetType().IsAssignableFrom(typeof(MollieStatusDetails)))
            {
                var mollieProvider = _paymentProviderFactory.GetPaymentProvider(PaymentProvider.Mollie);

                var details = (MollieStatusDetails)lastAttempt.StatusDetails;
                var paymentResult = (MollieGetPaymentResponse)await mollieProvider.GetPayment(details.MolliePaymentId);

                if (paymentResult.Status == PaymentProviderConstants.Mollie.Webhook.Paid)
                {
                    payment.PaymentStatus = PaymentStatus.Completed;
                    payment.Updated = DateTime.UtcNow;
                    lastAttempt.AttemptStatus = AttemptStatus.PaymentIsSuccessful;
                    lastAttempt.IsActive = false;
                }
                else // if (paymentResult.IsCancelable)
                {
                    payment.Updated = DateTime.UtcNow;
                    lastAttempt.IsActive = false;
                    lastAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled;

                    await mollieProvider.CancelPayment(details.MolliePaymentId); // case with failing is unreachable if webhooks works properly
                    return true;
                }
            }

            if (lastAttempt.StatusDetails.GetType().IsAssignableFrom(typeof(NetsStatusDetails)))
            {
                var netsProvider = _paymentProviderFactory.GetPaymentProvider(PaymentProvider.Nets);
                var details = (NetsStatusDetails)lastAttempt.StatusDetails;

                if (details.WebhookStatus == PaymentProviderConstants.Nets.Webhooks.ChargeCreated)
                {
                    payment.PaymentStatus = PaymentStatus.Completed;
                    payment.Updated = DateTime.UtcNow;
                    lastAttempt.AttemptStatus = AttemptStatus.PaymentIsSuccessful;
                    lastAttempt.IsActive = false;
                }
                else
                {
                    payment.Updated = DateTime.UtcNow;
                    lastAttempt.IsActive = false;
                    lastAttempt.AttemptStatus = AttemptStatus.RejectedEitherCancelled;

                    await netsProvider.CancelPayment(details.PaymentCheckoutId); // case with failing is unreachable if webhooks works properly
                    return true;
                }
            }

            return false;
        }
    }
}
