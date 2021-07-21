using BccPay.Core.Infrastructure.Enumerations;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Payments
{
    public interface IPaymentProvider
    {
        PaymentProvider PaymentProvider { get; }
        Task HandlePayment();
        Task RetrievePayment(string paymentId);
    }
}
