using BccPay.Core.Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.BackgroundServices
{
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
                    var _currencyExchangeService = (ICurrencyExchangeService)serviceProvider
                        .ServiceProvider
                        .GetRequiredService(typeof(ICurrencyExchangeService));

                    await _currencyExchangeService.UpsertCurrencyRate(cancellationToken);
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                }
                await Task.Delay(TimeSpan.FromMinutes(60), cancellationToken);
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
}
