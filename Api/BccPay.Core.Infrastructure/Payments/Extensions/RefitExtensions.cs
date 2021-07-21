using BccPay.Core.Infrastructure.Payments.Implementations.Providers;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;

namespace BccPay.Core.Infrastructure.Payments.Extensions
{
    public static class RefitExtensions
    {
        public static IServiceCollection AddRefitClients(this IServiceCollection services)
        {
            services.AddRefitClient<IRefitNetsClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://test.api.dibspayment.eu/v1/payments")); // TODO: ADD AuthHeaderHandler to set up headers

            return services;
        }
    }
}
