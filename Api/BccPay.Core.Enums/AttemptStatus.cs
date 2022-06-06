namespace BccPay.Core.Enums;

public enum AttemptStatus
{
    Processing,
    WaitingForCharge,
    RefundedInitiated,

    Failed,
    Canceled,
    Expired,
    PaidSucceeded,
    RefundedSucceeded
}
