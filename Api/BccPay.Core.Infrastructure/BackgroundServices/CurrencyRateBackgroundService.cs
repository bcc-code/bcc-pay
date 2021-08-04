using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.RefitClients;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.BackgroundServices
{
    public class CurrencyRateBackgroundService : BackgroundService
    {
        private readonly IFixerClient _fixerClient;
        private readonly IAsyncDocumentSession _documentSession;

        public CurrencyRateBackgroundService(
            IFixerClient fixerClient,
            IAsyncDocumentSession documentSession)
        {
            _fixerClient = fixerClient
                ?? throw new ArgumentNullException(nameof(fixerClient));
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _fixerClient.GetLatestCurrencyRate(Currencies.NOK.ToString(), new List<string>
                    {
                        Currencies.EUR.ToString(),
                        Currencies.USD.ToString()
                    });



                    await Task.Delay(TimeSpan.FromMinutes(60), cancellationToken);
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
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
