using System;
using BccPay.Core.Shared.Helpers;
using Bia.Countries.Iso3166;

namespace BccPay.Core.Shared.Converters
{
    public static class AddressConverter
    {
        /// <summary>
        /// Temp solution TODO: return value in accordance to payment provider requirements
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="countryCodeFormat"></param>
        /// <returns></returns>
        public static string ConvertCountry(string inputValue, CountryCodeFormat countryCodeFormat = CountryCodeFormat.Alpha3)
        {
            try
            {
                Country countryInformation = null;

                switch (inputValue.Length)
                {
                    case 2:
                        {
                            var normalize = inputValue.ToUpper();
                            countryInformation = Countries.GetCountryByAlpha2(normalize);
                            break;
                        }
                    case 3:
                        {
                            var normalize = inputValue.ToUpper();
                            countryInformation = Countries.GetCountryByAlpha3(normalize);
                            break;
                        }
                }

                if (int.TryParse(inputValue, out int countryCodeNumeric))
                    countryInformation = Countries.GetCountryByNumeric(countryCodeNumeric);

                if (inputValue.Length >= 4)
                    countryInformation = Countries.GetCountryByShortName(inputValue);

                if (countryInformation is null)
                    return "default";
                
                return countryCodeFormat switch
                {
                    CountryCodeFormat.Alpha2 => countryInformation?.Alpha2.ToString(),
                    CountryCodeFormat.Alpha3 => countryInformation?.Alpha3.ToString(),
                    CountryCodeFormat.Numeric => countryInformation?.Numeric.ToString(),
                    CountryCodeFormat.ShortName => countryInformation?.ShortName,
                    _ => countryInformation?.Alpha3.ToString(),
                };
            }
            catch
            {
                throw new Exception($"Unable to identify country by {inputValue}");
            }
        }
    }
}