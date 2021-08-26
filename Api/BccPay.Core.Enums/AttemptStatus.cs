namespace BccPay.Core.Enums
{
    public enum AttemptStatus
    {
        /// <summary>
        /// Initial status, waiting for action
        /// </summary>
        Processing,
        /// <summary>
        /// Waiting for automatic/manually payment charge
        /// </summary>
        WaitingForCharge,
        RejectedEitherCancelled,
        Successful,
        /// <summary>
        /// In case of expiration period given by the provider of checkout/payment
        /// </summary>
        Expired,
        /// <summary>
        /// Refunded on the provider side, possible through the dashboard on concrete provider site using admin tools
        /// </summary>
        RefundedSucceeded
    }
}
