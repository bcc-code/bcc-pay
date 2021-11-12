using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations
{
    internal class BaseRequestBuilder
    {
        protected string GetRedirectUrl(bool isMobile, string forMobile, string forBrowser, string paymentId = null, string attemptId = null)
        {
            string schema = isMobile
                ? forMobile
                : forBrowser;

            var result = ProcessPlaceholders<ReplaceModel>(schema, new ReplaceModel { AttemptId = attemptId, PaymentId = paymentId }, new(@"\{\{\s*(?<token>\w+)\s*\}\}"));

            return result;
        }

        protected static string GetLocale(string browserLanguage)
        {
            if (browserLanguage != "de-DE" && browserLanguage != "nb-NO")
                return "en-US";
            return browserLanguage;
        }

        private static string ProcessPlaceholders<T>(string text, T model, Regex pattern)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            MatchCollection matches = pattern.Matches(text);
            IEnumerable<string> tokens = matches.Cast<Match>().Select(m => m.Groups["token"].Value).Distinct();
            var publicProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var token in tokens)
            {
                var property = publicProperties.FirstOrDefault(p => string.Equals(p.Name, token, StringComparison.InvariantCultureIgnoreCase));
                if (property is not null && property.CanRead)
                {
                    object valueToReplace = property.GetValue(model) ?? string.Empty;
                    Regex replaceRegex = new(@"\{\{\s*" + token + @"\s*\}\}");
                    text = replaceRegex.Replace(text, valueToReplace.ToString());
                }
            }

            return text;
        }

        private class ReplaceModel
        {
            public string PaymentId { get; set; }
            public string AttemptId { get; set; }
        }
    }
}