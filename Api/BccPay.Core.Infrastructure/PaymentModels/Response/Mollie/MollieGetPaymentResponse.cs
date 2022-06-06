using System;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.MollieNodes;

namespace BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;

public class MollieGetPaymentResponse : IPaymentResponse
{
    public string Resource { get; set; }
    public string Id { get; set; }
    public string Mode { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public object Method { get; set; }
    public string Status { get; set; }
    public bool IsCancelable { get; set; }
    public string Locale { get; set; }
    public string RestrictPaymentMethodsToCountry { get; set; }
    public DateTime ExpiresAt { get; set; }
    public object Details { get; set; }
    public string ProfileId { get; set; }
    public string SequenceType { get; set; }
    public string RedirectUrl { get; set; }
    public string WebhookUrl { get; set; }

    public MollieLinks Links { get; set; }
    public MollieAmount Amount { get; set; }
    public MollieAmount AmountRefunded { get; set; }


    public bool IsSuccess { get; set; }
    public string Error { get; set; }
}
