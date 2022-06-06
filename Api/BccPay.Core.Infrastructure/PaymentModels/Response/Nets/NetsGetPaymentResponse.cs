using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

namespace BccPay.Core.Infrastructure.PaymentModels.Response.Nets;

public class NetsGetPaymentResponse : IPaymentResponse
{
    public bool IsSuccess { get; set; }
    public string Error { get; set; }
    public Summary Summary { get; set; }
    public OrderDetails Order { get; set; }
    public CheckoutOnRetrieve Checkout { get; set; }
}
