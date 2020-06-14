using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotLib;
using ExchangeClientLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BinanceBotSvc
{
    // sc create BinanceBotSvc binPath="C:\Users\mike\source\repos\McoreD\BinanceBot\BinanceBotSvc\bin\Debug\netcoreapp3.1\BinanceBotSvc.exe" obj=".\mike" password="password" start=auto
    // sc start BinanceBotSvc
    // sc delete BinanceBotSvc

    public class Worker : BackgroundService
    {
        private Bot _bot;

        public Worker(ILogger<Worker> logger)
        {
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _bot = new Bot();
                _bot.Start();

                _bot.Strategy.TradeListItemHandled += Strategy_TradeListItemHandled;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("BinanceBotSvc", ex.StackTrace, EventLogEntryType.Error);
            }

            await base.StartAsync(cancellationToken);
        }

        private void Strategy_TradeListItemHandled(TradingData data)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Market Price: {data.Price}");
            sb.AppendLine($"Entry Price: {data.BuyPriceAfterFees}");
            if (data.PriceLongBelow > 0)
            {
                sb.AppendLine($"Long Below: {data.PriceLongBelow}");
                sb.AppendLine($"Short Above: {data.PriceShortAbove}");
                sb.AppendLine($"Target Profit: {data.ProfitTarget}");
            }

            EventLog.WriteEntry("BinanceBotSvc", sb.ToString(), EventLogEntryType.Information);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}