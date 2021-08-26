namespace BccPay.Core.Enums
{
    public enum AttemptStatus
    {
        Processing,
        WaitingForCharge,
        RejectedEitherCancelled,
        Successful,
        Expired,
        RefundedSucceeded
    }
}
