using System.ComponentModel;

namespace BccPay.Core.Enums;

public enum PaymentMethod
{
    [Description("Credit Card Or Vipps (mobile)")]
    CreditCardOrVipps,
    [Description("Giropay")]
    Giropay,
    [Description("iDeal")]
    iDeal
}
