namespace BccPay.Core.Infrastructure.Dtos;

public interface IPaymentResponse
{
    public bool IsSuccess { get; set; }
    public string Error { get; set; }
}
