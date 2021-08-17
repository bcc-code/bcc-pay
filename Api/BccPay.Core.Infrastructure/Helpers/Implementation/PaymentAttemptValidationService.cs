using System.Linq;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using BccPay.Core.Infrastructure.PaymentProviders;
using Microsoft.Extensions.Logging;
using Refit;

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
            try
            {
                var lastAttempt = payment.Attempts.LastOrDefault();

                if (lastAttempt.StatusDetails.GetType().IsAssignableFrom(typeof(MollieStatusDetails)))
                {
                    var mollieProvider = _paymentProviderFactory.GetPaymentProvider(PaymentProvider.Mollie);

                    var details = (MollieStatusDetails)lastAttempt.StatusDetails;
                    var paymentResult = (MollieGetPaymentResponse)await mollieProvider.GetPayment(details.MolliePaymentId);
                    if (paymentResult.IsCancelable)
                    {
                        lastAttempt.IsActive = false;
                        await mollieProvider.CancelPayment(details.MolliePaymentId);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (lastAttempt.StatusDetails.GetType().IsAssignableFrom(typeof(NetsStatusDetails)))
                {
                    var netsProvider = _paymentProviderFactory.GetPaymentProvider(PaymentProvider.Nets);
                    var details = (NetsStatusDetails)lastAttempt.StatusDetails;
                    if (string.IsNullOrWhiteSpace(details.WebhookStatus))
                    {
                        lastAttempt.IsActive = false;
                        await netsProvider.CancelPayment(details.PaymentCheckoutId);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            catch (ApiException exception)
            {
                _logger.LogError(exception, nameof(TryCancelPreviousPaymentAttempt));
                return false;
            }
        }
    }
}
