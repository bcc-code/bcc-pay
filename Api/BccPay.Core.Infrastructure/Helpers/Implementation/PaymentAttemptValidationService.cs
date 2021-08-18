using System;
using System.Linq;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using BccPay.Core.Infrastructure.PaymentProviders;

namespace BccPay.Core.Infrastructure.Helpers.Implementation
{
    public class PaymentAttemptValidationService : IPaymentAttemptValidationService
    {
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public PaymentAttemptValidationService(IPaymentProviderFactory paymentProviderFactory)
        {
            _paymentProviderFactory = paymentProviderFactory;
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

                    var mollieCancelDetails = await mollieProvider.CancelPayment(details); // case with failing is unreachable if webhooks works properly
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

                    var netsCancelDetails = await netsProvider.CancelPayment(details); // case with failing is unreachable if webhooks works properly
                    return true;
                }
            }
            return false;
        }
    }
}
