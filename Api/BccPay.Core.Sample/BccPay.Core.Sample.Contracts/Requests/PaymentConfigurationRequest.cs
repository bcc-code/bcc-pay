namespace BccPay.Core.Sample.Contracts.Requests
{
    public class PaymentConfigurationRequest
    {
        public string[] CountryCodes { get; set; }
        public string[] PaymentTypes { get; set; }
    }
}
