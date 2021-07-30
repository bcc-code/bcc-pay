using BccPay.Core.Shared.Helpers;
using Bia.Countries.Iso3166;
using System;

namespace BccPay.Core.Shared.Converters
{
    public static class AddressConverter
    {
        /// <summary>
        /// Temp solution TODO: return value in accordance to payment provider requirements
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string ConvertCountry(string inputValue, CountryCodeFormat countryCodeFormat = CountryCodeFormat.Alpha3)
        {
            try
            {
                Country countryInformation;

                if (inputValue.Length == 2)
                    countryInformation = Countries.GetCountryByAlpha2(inputValue);
                else if (inputValue.Length == 3)
                    countryInformation = Countries.GetCountryByAlpha3(inputValue);
                else if (int.TryParse(inputValue, out int countryCodeNumeric))
                    countryInformation = Countries.GetCountryByNumeric(countryCodeNumeric);
                else
                    countryInformation = Countries.GetCountryByShortName(inputValue);


                return countryCodeFormat switch
                {
                    CountryCodeFormat.Alpha2 => countryInformation.Alpha2.ToString(),
                    CountryCodeFormat.Alpha3 => countryInformation.Alpha3.ToString(),
                    CountryCodeFormat.Numeric => countryInformation.Numeric.ToString(),
                    _ => countryInformation.Alpha3.ToString(),
                };
            }
            catch
            {
                throw new Exception($"Unable to identify country by {inputValue}");
            }
        }
    }
}
