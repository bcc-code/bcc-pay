using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
using BccPay.Core.Infrastructure.PaymentProviders.RefitClients;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations
{
    internal class NetsCreditCardPaymentProvider : IPaymentProvider
    {
        private readonly INetsClient _netsClient;
        private readonly NetsProviderOptions _options;
        private readonly IDictionary<string, string> _headers;

        public NetsCreditCardPaymentProvider(INetsClient netsClient, NetsProviderOptions options)
        {
            _netsClient = netsClient
                ?? throw new ArgumentNullException(nameof(netsClient));

            _options = options;

            _headers = new Dictionary<string, string>
            {
                { PaymentProviderConstants.AuthorizationHeader, _options.SecretKey },
                { PaymentProviderConstants.ContentType, MediaTypeNames.Application.Json }
            };
        }

        public string PaymentMethod => Enums.PaymentMethod.NetsCreditCard.ToString();

        public async Task<object> CreatePayment(PaymentRequestDto paymentRequest)
        {
            try
            {
                int amountMonets = Convert.ToInt32(paymentRequest.Amount * 100);

                var result = await _netsClient.CreatePaymentAsync(_headers, new NetsPaymentRequest()
                {
                    Checkout = new CheckoutOnCreate
                    {
                        IntegrationType = PaymentProviderConstants.Nets.IntegrationType,
                        Charge = false,
                        MerchantHandlesConsumerData = true,
                        Url = _options.CheckoutPageUrl,
                        TermsUrl = _options.TermsUrl,
                        Consumer = new ConsumerOnCreate
                        {
                            Email = paymentRequest.Email,
                            PhoneNumber = new PhoneNumber
                            {
                                Prefix = paymentRequest.PhoneNumberPrefix,
                                Number = paymentRequest.PhoneNumberBody
                            },
                            PrivatePerson = new PrivatePersonOnCreate
                            {
                                FirstName = paymentRequest.FirstName,
                                LastName = paymentRequest.LastName
                            },
                            ShippingAddress = new Address
                            {
                                AddressLine1 = paymentRequest.Address.AddressLine1,
                                AddressLine2 = paymentRequest.Address.AddressLine2,
                                City = paymentRequest.Address.City,
                                Country = paymentRequest.Address.Country,
                                PostalCode = paymentRequest.Address.PostalCode
                            }
                        }
                    },
                    Order = new Order
                    {
                        Amount = amountMonets,
                        Currency = paymentRequest.Currency,
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Reference = PaymentProviderConstants.Nets.ItemReference,
                                Name = $"DONATION-{paymentRequest.Amount}",
                                Quantity = 1, // static  
                                Unit = PaymentProviderConstants.Nets.ItemUnit,
                                UnitPrice = amountMonets, // The price per unit excluding VAT.
                                GrossTotalAmount = amountMonets, //The total amount including VAT (netTotalAmount + taxAmount).
                                NetTotalAmount = amountMonets //The total amount excluding VAT (unitPrice * quantity).
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

                return new NetsStatusDetails
                {
                    IsSuccessful = true,
                    PaymentCheckoutId = result.PaymentId
                };
            }
            catch (ApiException exception)
            {
                return new NetsStatusDetails
                {
                    IsSuccessful = false,
                    Error = exception?.Content
                };
                //throw new ExternalApiCallException(HttpStatusCode.BadRequest, exception?.Content);
            }
        }
    }
}
