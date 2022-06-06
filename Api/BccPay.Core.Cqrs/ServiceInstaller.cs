using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Domain.Entities;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceInstaller
{
    public static IServiceCollection ConfigureBccCoreCqrs(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreatePaymentCommand).Assembly);

        services.AddTransient(typeof(IRequestHandler<CreatePaymentAttemptCommand, IStatusDetails>), typeof(CreatePaymentAttemptCommandHandler));

        return services;
    }
}
