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

            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));

            _headers = new Dictionary<string, string>
            {
                { PaymentProviderConstants.AuthorizationHeader, _configuration[PaymentProviderConstants.Nets.SecretKey] }, // TODO: move to azure secrets
                { PaymentProviderConstants.ContentType, MediaTypeNames.Application.Json }
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
                        IntegrationType = PaymentProviderConstants.Nets.IntegrationType,
                        Charge = false,
                        MerchantHandlesConsumerData = false,
                        Url = PaymentProviderConstants.Nets.CheckoutPageUrl, // TODO: Specifies where the checkout will be loaded if using an embedded checkout page. See also the integrationType property.
                        TermsUrl = PaymentProviderConstants.Nets.TermsUrl // TODO: terms link is required
                    },
                    Order = new Order
                    {
                        Amount = paymentRequest.Amount,// + n // The total amount of the order including VAT, if any. (Sum of all grossTotalAmounts in the order.)
                        Currency = paymentRequest.Currency,
                        Items = new List<Item> // A list of order items. At least one item must be specified.
                        {
                            new Item
                            {
                                Reference = PaymentProviderConstants.Nets.ItemReference, // A reference to recognize the product, usually the SKU (stock keeping unit) of the product. For convenience in the case of refunds or modifications of placed orders, the reference should be unique for each variation of a product item (size, color, etc).
                                Name = $"DONATION-{paymentRequest.Amount}",
                                Quantity = 1, // static  
                                Unit = PaymentProviderConstants.Nets.ItemUnit, // The defined unit of measurement for the product, for example pcs, liters, or kg.
                                UnitPrice = paymentRequest.Amount, // The price per unit excluding VAT.
                                GrossTotalAmount = paymentRequest.Amount, //The total amount including VAT (netTotalAmount + taxAmount).
                                NetTotalAmount = paymentRequest.Amount //The total amount excluding VAT (unitPrice * quantity).
                            }
                        }
                    }

                    // TODO: WEBHOOKS
                    // Notifications = new Notifications
                    // {
                    //     Webhooks = new List<Webhook>
                    //     {
                    //         new Webhook
                    //         {
                    //             Authorization = "TOKEN",
                    //             EventName = PaymentProviderConstants.Nets.WebHookEventName,
                    //             Url = PaymentProviderConstants.Nets.WebHookUrl
                    //         }
                    //     }
                    // },

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
