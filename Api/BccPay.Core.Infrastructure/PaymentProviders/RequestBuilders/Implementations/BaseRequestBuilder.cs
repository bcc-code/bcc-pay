using System.Text.RegularExpressions;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations
{
    internal class BaseRequestBuilder
    {
        protected string GetRedirectUrl(bool isMobile, string forMobile, string forBrowser, string identifier = null)
        {
            string schema = isMobile
                ? forMobile
                : forBrowser;

            return string.IsNullOrWhiteSpace(identifier)
                ? (Regex.Match(schema, @"^.*?(?=/{)")).Value
                : Regex.Replace(schema, "{.*?}", identifier);
        }

        protected static string GetLocale(string browserLanguage)
        {
            if (browserLanguage != "de-DE" && browserLanguage != "nb-NO")
                return "en-US";
            return browserLanguage;
        }
    }
}