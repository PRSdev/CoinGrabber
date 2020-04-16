using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace BinanceBotLib
{
    public static class Bot
    {
        #region IO

        public static readonly string PersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BinanceBot");
        public static Settings Settings { get; private set; }
        public static string SettingsFilePath
        {
            get
            {
                return Path.Combine(PersonalFolder, "Settings.json");
            }
        }

        public static string LogFilePath
        {
            get
            {
                string logsFolder = Path.Combine(PersonalFolder, "Logs");
                string filename = string.Format("BinanceBot-Log-{0:yyyy-MM}.log", DateTime.Now);
                return Path.Combine(logsFolder, filename);
            }
        }
        private static Logger logger = new Logger(Bot.LogFilePath);

        public static void LoadSettings()
        {
            Settings = Settings.Load(SettingsFilePath);
        }

        public static void SaveSettings()
        {
            if (Settings != null)
            {
                Settings.Save(SettingsFilePath);
            }
        }

        public static void WriteLog(string message)
        {
            Console.WriteLine(message);
            logger.WriteLine(message);
        }

        #endregion IO

        private static bool _init = false;

        private static readonly ExchangeType _exchangeType = ExchangeType.BinanceExchange;
        private static System.Timers.Timer _marketTimer = new System.Timers.Timer();
        public static Strategy Strategy { get; private set; }

        public static void Init()
        {
            switch (Bot.Settings.BotMode)
            {
                case BotMode.FixedProfit:
                    _marketTimer.Interval = MathHelpers.Random(60, 120) * 1000; // Randomly every 1-2 minutes (60-120)
                    Strategy = new FixedProfitStrategy(_exchangeType);
                    break;

                case BotMode.FixedPriceChange:
                    _marketTimer.Interval = MathHelpers.Random(60, 120) * 1000; // Randomly every 1-2 minutes (60-120)
                    Strategy = new FixedPriceChangeStrategy(_exchangeType);
                    break;

                case BotMode.TradingViewSignal:
                    _marketTimer.Interval = 5000; // Every 5 seconds
                    Strategy = new TradingViewAlertStrategy(_exchangeType);
                    break;

                default:
                    Console.WriteLine("Unhandled Bot Mode.");
                    Console.ReadLine();
                    return;
            }

            _init = true;
        }

        public static void Start()
        {
            if (!_init) Init();

#if DEBUG
            Strategy.Trade();
#endif
            _marketTimer.Elapsed += MarketTimer_Tick;
            _marketTimer.Start();
        }

        public static void Stop()
        {
            _marketTimer.Stop();
        }

        private static void MarketTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(Bot.Settings.APIKey))
                throw new Exception("Settings reset!");

            Strategy.Trade();
            NativeMethods.PreventSleep();
        }
    }
}