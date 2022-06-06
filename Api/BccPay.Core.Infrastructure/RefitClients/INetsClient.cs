using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
using BccPay.Core.Infrastructure.PaymentModels.Response.Nets;
using Refit;

namespace BccPay.Core.Infrastructure.RefitClients;

internal interface INetsClient
{
    [Post("/v1/payments")]
    Task<NetsCreatePaymentResponse> CreatePaymentAsync([HeaderCollection] IDictionary<string, string> headers, [Body] NetsPaymentRequest payment);

    [Get("/v1/payments/{paymentId}")]
    Task<NetsRetrievePaymentResponse> RetrievePayment([HeaderCollection] IDictionary<string, string> headers, [AliasAs("paymentId")] string paymentId);

    [Post("/v1/payments/{paymentId}/charges")]
    Task<NetsChargeResponse> ChargePayment([HeaderCollection] IDictionary<string, string> headers, [AliasAs("paymentId")] string paymentId, [Body] NetsChargeRequest chargeRequest);

    [Put("/v1/payments/{paymentId}/terminate")]
    Task<HttpResponseMessage> TerminatePayment([HeaderCollection] IDictionary<string, string> headers, [AliasAs("paymentId")] string paymentId);
}

