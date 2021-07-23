using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
using BccPay.Core.Infrastructure.PaymentProviders.RefitClients;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations
{
    public class CreditCardPaymentProvider : IPaymentProvider
    {
        private readonly INetsClient _netsClient;
        private readonly IConfiguration _configuration;
        private readonly IDictionary<string, string> _headers;

        public CreditCardPaymentProvider(INetsClient netsClient,
            IConfiguration configuration)
        {
            _netsClient = netsClient
                ?? throw new ArgumentNullException(nameof(netsClient));

            _configuration = configuration;
            _headers = new Dictionary<string, string>
            {
                {"Authorization", _configuration[PaymentProviderConstants.NetsSecretKey] }, // TODO: move to azure secrets
                {"content-type", MediaTypeNames.Application.Json }
            };
        }

        public string PaymentMethod => Enums.PaymentMethod.CreditCard.ToString();

        public async Task<string> CreatePayment(PaymentRequestDto paymentRequest)
        {
            try
            {
                var result = await _netsClient.CreatePaymentAsync(_headers, new NetsPaymentRequest()
                {
                    Checkout = new CheckoutOnCreate
                    {
                        IntegrationType = "EmbeddedCheckout",
                        Charge = false,
                        MerchantHandlesConsumerData = false,
                        Url = "https://localhost:5001/Payment/Callback",
                        TermsUrl = "https://localhost:5001/Payment/Terms"
                    },
                    // TODO: WEBHOOKS
                    // Notifications = new Notifications
                    // {
                    //     Webhooks = new List<Webhook>
                    //     {
                    //         new Webhook
                    //         {
                    //             Authorization = "TOKEN",
                    //             EventName = "payment.created",
                    //             Url = "api/v1/samvirk/callback"
                    //         }
                    //     }
                    // },
                    Order = new Order
                    {
                        Amount = paymentRequest.Amount,
                        Currency = paymentRequest.Currency,
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Reference = "reference-name",
                                Name = "donate-1500",
                                Unit = "currency",
                                UnitPrice = paymentRequest.Amount,
                                GrossTotalAmount = paymentRequest.Amount,
                                NetTotalAmount = paymentRequest.Amount,
                                Quantity = 1 // static                                 
                            }
                        }
                    }

                });

                return result.PaymentId;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                throw;
            }
        }
    }
}
