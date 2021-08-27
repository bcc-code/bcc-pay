namespace BccPay.Core.Enums
{
    public enum AttemptStatus
    {
        Processing,
        WaitingForCharge,
        RefundedInitiated,

        Failed,
        Cancelled,
        Expired,
        PaidSucceeded,
        RefundedSucceeded
    }
}
