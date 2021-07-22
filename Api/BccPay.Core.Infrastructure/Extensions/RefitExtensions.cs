using BccPay.Core.Infrastructure.PaymentProviders.RefitClients;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;

namespace BccPay.Core.Infrastructure.Payments.Extensions
{
    public static class RefitExtensions
    {
        public static IServiceCollection AddRefitClients(this IServiceCollection services)
        {
            services.AddRefitClient<INetsClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://test.api.dibspayment.eu/v1/payments")); // TODO: ADD AuthHeaderHandler to set up headers

            return services;
        }
    }
}
