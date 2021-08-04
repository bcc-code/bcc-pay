using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
using BccPay.Core.Infrastructure.PaymentProviders.RefitClients;
using Microsoft.Net.Http.Headers;
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
                { HeaderNames.Authorization, _options.SecretKey },
                { HeaderNames.ContentType, MediaTypeNames.Application.Json }
            };
        }

        public string PaymentMethod => Enums.PaymentMethod.NetsCreditCard.ToString();

        public async Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest)
        {
            try
            {
                var result = await _netsClient.CreatePaymentAsync(_headers, BuildNetsPaymentRequest(paymentRequest));

                return new NetsStatusDetails
                {
                    IsSuccessful = true,
                    PaymentCheckoutId = result.PaymentId
                };
            }
            catch (ApiException retryException)
            {
                try
                {
                    var result = await _netsClient.CreatePaymentAsync(_headers, BuildNetsPaymentRequest(paymentRequest, false));
                    return new NetsStatusDetails
                    {
                        IsSuccessful = true,
                        PaymentCheckoutId = result.PaymentId,
                        Error = "{\"notValidUserBillingDataInTheSystem\":" + retryException?.Content + "}"
                    };
                }
                catch (ApiException exception)
                {
                    return new NetsStatusDetails
                    {
                        IsSuccessful = false,
                        Error = exception?.Content
                    };
                }
            }
        }

        private NetsPaymentRequest BuildNetsPaymentRequest(PaymentRequestDto paymentRequest, bool IsUserDataValid = true)
        {
            int amountMonets = Convert.ToInt32(paymentRequest.Amount * 100);
            List<Webhook> webhooks = new();

            foreach (var webhookEvent in PaymentProviderConstants.Nets.Webhooks.Messages)
            {
                webhooks.Add(new Webhook
                {
                    Authorization = paymentRequest.NotificationAccessToken,
                    EventName = webhookEvent.Key,
                    Url = _options.NotificationUrl
                });
            }

            Notifications notifications = new()
            {
                Webhooks = webhooks
            };

            Order order = new()
            {
                Amount = amountMonets,
                Currency = paymentRequest.Currency,
                Items =
                    new List<Item>
                    {
                        new Item
                        {
                            Reference = PaymentProviderConstants.Nets.Order.ItemReference,
                            Name = $"DONATION-{paymentRequest.Amount}",
                            Quantity = 1,
                            Unit = PaymentProviderConstants.Nets.Order.ItemUnit,
                            UnitPrice = amountMonets,
                            GrossTotalAmount = amountMonets,
                            NetTotalAmount = amountMonets
                        }
                    }
            };

            if (IsUserDataValid)
            {
                return new NetsPaymentRequest()
                {
                    Checkout = new CheckoutOnCreate
                    {
                        IntegrationType = PaymentProviderConstants.Nets.Order.IntegrationType,
                        Charge = true,
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
                    Order = order,
                    Notifications = notifications,
                };
            }
            else
            {
                return new NetsPaymentRequest()
                {
                    Checkout =
                        new CheckoutOnCreate
                        {
                            IntegrationType = PaymentProviderConstants.Nets.Order.IntegrationType,
                            Charge = false,
                            MerchantHandlesConsumerData = false,
                            Url = _options.CheckoutPageUrl,
                            TermsUrl = _options.TermsUrl,
                        },
                    Order = order,
                    Notifications = notifications,
                };
            }
        }
    }
}
