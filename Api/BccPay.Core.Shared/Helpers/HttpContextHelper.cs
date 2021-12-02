using Microsoft.AspNetCore.Http;

namespace BccPay.Core.Shared.Helpers
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        private static HttpContext Current 
            => _httpContextAccessor.HttpContext;

        public static string AppBaseUrl => Current.Request.Headers["Referer"].ToString();

        public static void Configure(IHttpContextAccessor contextAccessor)
            => _httpContextAccessor = contextAccessor;
    }
}