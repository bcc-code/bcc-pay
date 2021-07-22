using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProvider
    {
        Task<string> CreatePayment();
    }
}
