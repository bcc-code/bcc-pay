using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BccPay.Core.Infrastructure.BackgroundServices;

public class CurrencyRateUpdateBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    public CurrencyRateUpdateBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider
            ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var serviceProvider = _serviceProvider.CreateScope();
                var _currencyService = (ICurrencyService)serviceProvider
                    .ServiceProvider
                    .GetRequiredService(typeof(ICurrencyService));

                await _currencyService.UpsertByBaseCurrencyRate(Enums.Currencies.NOK, cancellationToken);
            }
            catch
            {
                await Task.Delay(TimeSpan.FromMinutes(60), cancellationToken);
            }
            await Task.Delay(TimeSpan.FromMinutes(120), cancellationToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            await ExecuteAsync(cancellationToken);
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
