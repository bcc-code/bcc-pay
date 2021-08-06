namespace BccPay.Core.Domain.Entities
{
    public class Country
    {
        public static string GetCountryId(string countryCode) => $"countries/{countryCode}";

        public string Id => GetCountryId(CountryCode);

        public string CountryCode { get; set; }

        public string[] PaymentConfigurations { get; set; }
    }
}
