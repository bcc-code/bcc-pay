using Microsoft.AspNetCore.Http;

namespace BccPay.Core.Shared.Helpers
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        private static HttpContext Current 
            => _httpContextAccessor.HttpContext;

        public static string AppBaseUrl =>
            $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

        public static void Configure(IHttpContextAccessor contextAccessor)
            => _httpContextAccessor = contextAccessor;
    }
}