using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;

namespace BccPay.Core.Infrastructure.Helpers
{
    public interface IPaymentAttemptValidationService
    {
        Task<bool> TryCancelPreviousPaymentAttempt(Payment payment);
    }
}
