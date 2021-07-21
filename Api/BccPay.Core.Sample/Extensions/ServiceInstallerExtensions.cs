using BccPay.Core.Infrastructure.Payments;
using BccPay.Core.Infrastructure.Payments.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace BccPay.Core.Sample.Extensions
{
    public static class ServiceInstallerExtensions
    {
        public static IServiceCollection AddBccPaymentServices(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentHandler, CommonPaymentHandler>();

            return RegisterAllTypes<IPaymentProvider>(services, new[] { typeof(ServiceInstallerExtensions).Assembly });
        }

        private static IServiceCollection RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            static bool exp(TypeInfo x) => x.GetInterfaces().Contains(typeof(T)) && !x.IsGenericType && !x.IsInterface;
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(exp));
            foreach (var type in typesFromAssemblies)
            {
                var interfaceType = type.GetInterfaces()
                    .FirstOrDefault(i => !i.IsGenericType && i != typeof(T) && typeof(T).IsAssignableFrom(i))
                    ?? typeof(T);
                services.Add(new ServiceDescriptor(interfaceType, type, lifetime));
            }

            return services;
        }
    }
}
