using System;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BccPay.Core.Sample.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
        => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;
            response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorResponse = new
            {
                message = exception?.Message
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);

            await response.WriteAsync(errorJson);
        }
    }
}
