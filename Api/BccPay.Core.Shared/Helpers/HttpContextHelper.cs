using System;
using Microsoft.AspNetCore.Http;

namespace BccPay.Core.Shared.Helpers
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        private static HttpContext Current
            => _httpContextAccessor.HttpContext;

        public static string AppReferrerUrl => ClearReferrer(Current.Request.Headers["Referer"].ToString());

        private static string ClearReferrer(string referer)
        {
            var url = new Uri(referer);
            return $"{url.Scheme}://{url.Host}";
        }

        public static void Configure(IHttpContextAccessor contextAccessor)
            => _httpContextAccessor = contextAccessor;
    }
}