using Binance.Net;
using Binance.Net.Objects;
using BinanceBotLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CryptoExchange.Net.Objects;

namespace BinanceBotConsole
{
    internal class Program

    {
        private static void Main(string[] args)
        {
            Bot.LoadSettings();

            // Error handling
            if (string.IsNullOrEmpty(Bot.Settings.APIKey))
            {
                Console.Write("Enter Binance API Key: ");
                Bot.Settings.APIKey = Console.ReadLine();

                Console.Write("Enter Binance Secret Key: ");
                Bot.Settings.SecretKey = Console.ReadLine();

                Bot.SaveSettings(); // Save API Key and Secret Key
            }

            // Choose Bot mode
            foreach (BotMode bm in Enum.GetValues(typeof(BotMode)))
            {
                Console.WriteLine($"{bm.GetIndex()} for {bm.GetDescription()}");
            }
            Console.Write("Choose Bot mode: ");

            int intMode;
            int.TryParse(Console.ReadLine(), out intMode);
            Bot.Settings.BotMode = (BotMode)intMode;

            // TODO: Settings summary for user

            switch (Bot.Settings.BotMode)
            {
                case BotMode.DayTrade:
                    if (Bot.Settings.DailyProfitTarget <= 0)
                    {
                        Console.WriteLine("Daily Profit Target must be greater than zero!");
                        Console.ReadLine();
                        return;
                    }
                    break;
            }

            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(Bot.Settings.APIKey, Bot.Settings.SecretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });

#if DEBUG
            TradingHelper.SwingTrade();
            Console.WriteLine();
#endif

            Bot.SaveSettings(); // Save settings before exiting

            Console.WriteLine($"{Bot.Settings.BotMode.GetDescription()} Bot initiated...");
            Bot.Start();

            Console.ReadLine();
        }
    }
}