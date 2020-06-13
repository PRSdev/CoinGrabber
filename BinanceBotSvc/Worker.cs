using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BinanceBotSvc
{
    // sc create BinanceBotSvc binPath="C:\Users\mike\source\repos\McoreD\BinanceBot\BinanceBotSvc\bin\Debug\netcoreapp3.1\BinanceBotSvc.exe" obj=".\mike" password="password" start=auto
    // sc start BinanceBotSvc
    // sc delete BinanceBotSvc

    public class Worker : BackgroundService
    {
        public static readonly string PersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BinanceBot");

        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            EventLog.WriteEntry("BinanceBotSvc", "Started", EventLogEntryType.Information);
            try
            {
                Bot bot = new Bot();
                _logger.LogInformation(bot.Settings.BotMode.ToString());
                bot.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("BinanceBotSvc", ex.StackTrace, EventLogEntryType.Information);
            }

            await base.StartAsync(cancellationToken);
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