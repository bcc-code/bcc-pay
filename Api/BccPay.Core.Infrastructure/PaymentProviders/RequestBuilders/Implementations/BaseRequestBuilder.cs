using System.Text.RegularExpressions;
using BccPay.Core.Shared.Helpers;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations;

internal class BaseRequestBuilder
{
    protected string GetRedirectUrl(
        string redirectUrl, 
        string paymentId = null,
        string attemptId = null)
        => TemplateHelper.ProcessPlaceholders(redirectUrl,
            new ReplaceModel {AttemptId = attemptId, PaymentId = paymentId, Host = HttpContextHelper.AppReferrerUrl},
            new Regex(@"\{\{\s*(?<token>\w+)\s*\}\}"));

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
        public string Host { get; set; }
    }
}
