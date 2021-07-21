using BccPay.Core.Infrastructure.Enumerations;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Payments
{
    public interface IPaymentHandler
    {
        PaymentProvider PaymentProvider { get; }
        Task ProcessPayment();
    }
}
