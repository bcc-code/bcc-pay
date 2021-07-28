namespace BccPay.Core.Enums
{
    public enum PaymentProgress
    {
        /// <summary>
        /// Not yet initiated
        /// </summary>
        Draft,
        /// <summary>
        /// The payment is being processed by the Payment microservice
        /// </summary>
        Processing,
        /// <summary>
        /// We are waiting for the payment to actually arrive and be confirmed. Used for asynchronous payment methods that can fail.
        /// </summary>
        Pending,
        /// <summary>
        /// The payment is failed
        /// </summary>
        Failed,
        /// <summary>
        /// The payment is finally confirmed
        /// </summary>
        Confirmed,
        /// <summary>
        /// The payment was cancelled and is not expected to be finalized
        /// </summary>
        Cancelled
    }
}
