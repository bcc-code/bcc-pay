namespace BccPay.Core.Contracts.Requests
{
    public class CreatePaymentRequest
    {
        public string PayerId { get; set; }
        /// <summary>
        /// For Nets provider : Currency should always be specified using a 3-letter code (ISO-4217). 
        /// </summary>
        public string Currency { get; set; }
        public string Country { get; set; }
        /// <summary>
        /// For Nets provider : Amounts are specified in the lowest monetary unit for the 
        /// given currency, without punctuation marks. For example: 100,00 NOK is specified 
        /// as 10000 and 9.99 USD is specified as 999. 
        /// </summary>
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }

        public Payer Payer { get; set; }
    }
}
