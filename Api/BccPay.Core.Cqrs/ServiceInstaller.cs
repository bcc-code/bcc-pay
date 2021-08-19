using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Cqrs.Commands.Webhooks;
using BccPay.Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BccPay.Core.Cqrs
{
    public static class ServiceInstaller
    {
        public static IServiceCollection ConfigureBccCoreCqrs(this IServiceCollection services)
        {
            services.AddMediatR(new System.Reflection.Assembly[]
            {
                typeof(CreatePaymentCommand).Assembly,
                typeof(CreatePaymentAttemptCommand).Assembly,
                typeof(UpdateNetsPaymentStatusCommand).Assembly,
                typeof(UpdateMolliePaymentStatusCommand).Assembly
            });

            services.AddTransient(typeof(IRequestHandler<CreatePaymentAttemptCommand, IStatusDetails>), typeof(CreatePaymentAttemptCommandHandler));

            return services;
        }
    }
}
