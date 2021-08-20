using System;
using System.Collections.Generic;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations
{
    internal class NetsCreditCardRequestBuilder : INetsPaymentRequestBuilder
    {
        private readonly NetsProviderOptions _options;

        public NetsCreditCardRequestBuilder(NetsProviderOptions options)
        {
            _options = options;
        }

        public NetsPaymentRequest BuildNetsPaymentRequest(PaymentRequestDto paymentRequest, string originUrl, bool IsUserDataValid = true)
        {
            int amountMonets = Convert.ToInt32(paymentRequest.Amount * 100);
            List<Webhook> webhooks = new();

            foreach (var webhookEvent in PaymentProviderConstants.Nets.Webhooks.Messages)
            {
                webhooks.Add(new Webhook
                {
                    Authorization = paymentRequest.NotificationAccessToken,
                    EventName = webhookEvent.Key,
                    Url = _options.NotificationUrl + paymentRequest.PaymentId
                });
            }

            PaymentModels.NetsNodes.Notifications notifications = new()
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
                        Url = $"{originUrl}{_options.CheckoutPageUrl}",
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
                            Url = $"{originUrl}{_options.CheckoutPageUrl}",
                            TermsUrl = _options.TermsUrl,
                        },
                    Order = order,
                    Notifications = notifications,
                };
            }
        }
    }
}
