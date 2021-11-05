using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations
{
    internal class NetsRequestBuilder : BaseRequestBuilder, INetsPaymentRequestBuilder
    {
        private readonly NetsProviderOptions _options;

        public NetsRequestBuilder(NetsProviderOptions options)
        {
            _options = options;
        }

        public NetsPaymentRequest BuildNetsPaymentRequest(PaymentRequestDto paymentRequest, string originUrl,
            bool isUserDataValid = true)
        {
            int amountInCoins = Convert.ToInt32(paymentRequest.Amount * 100);

            List<Webhook> webhooks = PaymentProviderConstants.Nets.Webhooks.Messages.Select(webhookEvent => new Webhook
            {
                Authorization = paymentRequest.NotificationAccessToken,
                EventName = webhookEvent.Key,
                Url = _options.NotificationUrl + $"/{paymentRequest.PaymentId}"
            }).ToList();

            PaymentModels.NetsNodes.Notifications notifications = new()
            {
                Webhooks = webhooks
            };

            Order order = new()
            {
                Amount = amountInCoins,
                Currency = paymentRequest.BaseCurrency,
                Items =
                    new List<Item>
                    {
                        new Item
                        {
                            Reference = PaymentProviderConstants.Nets.Order.ItemReference,
                            Name = $"DONATION-{paymentRequest.Amount}",
                            Quantity = 1,
                            Unit = PaymentProviderConstants.Nets.Order.ItemUnit,
                            UnitPrice = amountInCoins,
                            GrossTotalAmount = amountInCoins,
                            NetTotalAmount = amountInCoins
                        }
                    }
            };


            string integrationType = paymentRequest.IsHostedCheckout
                ? PaymentProviderConstants.Nets.IntegrationType.HostedPaymentPage
                : PaymentProviderConstants.Nets.IntegrationType.EmbeddedCheckout;
            string hostedReturnUrl = paymentRequest.IsHostedCheckout
                ? GetRedirectUrl(paymentRequest.IsMobile, _options.MobileReturnUrl, _options.ReturnUrl,
                    paymentRequest.UsePaymentIdAsRouteInRedirectUrl is null || false ? null : paymentRequest.PaymentId)
                : null;


            string embeddedUrl = paymentRequest.IsHostedCheckout ? null : $"{originUrl}{_options.CheckoutPageUrl}";

            if (isUserDataValid)
            {
                return new NetsPaymentRequest()
                {
                    Checkout = new CheckoutOnCreate
                    {
                        IntegrationType = integrationType,
                        Charge = true,
                        MerchantHandlesConsumerData = true,
                        Url = embeddedUrl,
                        ReturnUrl = hostedReturnUrl,
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
                            IntegrationType = integrationType,
                            ReturnUrl = hostedReturnUrl,
                            Charge = false,
                            MerchantHandlesConsumerData = false,
                            Url = embeddedUrl,
                            TermsUrl = _options.TermsUrl,
                        },
                    Order = order,
                    Notifications = notifications,
                };
            }
        }
    }
}