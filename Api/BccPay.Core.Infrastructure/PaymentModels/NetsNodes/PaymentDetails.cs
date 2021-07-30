namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    public class PaymentDetails
    {
        /// <summary>
        /// The type of payment. Possible values are: 'CARD', 'INVOICE', 'A2A', 'INSTALLMENT', 'WALLET', and 'PREPAID-INVOICE'.
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// The payment method, for example Visa or Mastercard.
        /// </summary>
        public string PaymentMethod { get; set; }
        public InvoiceDetails InvoiceDetails { get; set; }
        public CardDetails CardDetails { get; set; }
    }
}