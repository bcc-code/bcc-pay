using BccPay.Core.Shared.Helpers;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations
{
    internal class BaseRequestBuilder
    {
        protected string GetRedirectUrl(bool isMobile, string forMobile, string forBrowser, string paymentId = null, string attemptId = null)
        {
            string schema = isMobile
                ? forMobile
                : forBrowser;

            var result = TemplateHelper.ProcessPlaceholders<ReplaceModel>(schema, new ReplaceModel { AttemptId = attemptId, PaymentId = paymentId }, new(@"\{\{\s*(?<token>\w+)\s*\}\}"));

            return result;
        }

        protected static string GetLocale(string browserLanguage)
        {
            if (browserLanguage != "de-DE" && browserLanguage != "nb-NO")
                return "en-US";
            return browserLanguage;
        }

        private class ReplaceModel
        {
            public string PaymentId { get; set; }
            public string AttemptId { get; set; }
        }
    }
}