﻿namespace BccPay.Core.Domain.Entities
{
    public class Country
    {
        public const string DefaultCountryCode = "default";

        public static string GetDocumentId(string countryCode) => $"countries/{countryCode}";

        public string Id => GetDocumentId(CountryCode);

        public string CountryCode { get; set; }
    }
}
