using BccPay.Core.Shared.Helpers;

namespace BccPay.Core.Infrastructure.BccPaymentSettings
{
    public class InternalSettings
    {
        public CountryCodeFormat StoreCountryCodeFormat { get; set; } = CountryCodeFormat.Alpha3;
        public CountryCodeFormat DisplayCountryCodeFormat { get; set; } = CountryCodeFormat.Alpha3;
    }
}