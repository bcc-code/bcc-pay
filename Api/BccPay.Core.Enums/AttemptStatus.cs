namespace BccPay.Core.Enums
{
    public enum AttemptStatus
    {
        ProcessingPayment,
        WaitingForCharge,
        RejectedEitherCancelled,
        PaymentIsSuccessful,
        Expired
    }
}
