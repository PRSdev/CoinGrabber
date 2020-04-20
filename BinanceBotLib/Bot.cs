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
                return Path.Combine(PersonalFolder, "TradingView-Settings.json");
            }
        }

        public static string LogFilePath
        {
            get
            {
                string logsFolder = Path.Combine(PersonalFolder, "Logs");
                string filename = string.Format("TradingView-BinanceBot-Log-{0:yyyy-MM}.log", DateTime.Now);
                return Path.Combine(logsFolder, filename);
            }
        }
        private static Logger logger = new Logger(Bot.LogFilePath);

        public static Settings LoadSettings()
        {
            Settings = Settings.Load(SettingsFilePath);
            return Settings;
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

        public static readonly ExchangeType _exchangeType = ExchangeType.MockupExchange;
        private static System.Timers.Timer _marketTimer = new System.Timers.Timer();
        public static Strategy Strategy { get; private set; }

        public static void Init(Settings settings)
        {
            Settings = settings;

            double timerInterval = _exchangeType == ExchangeType.BinanceExchange ? MathHelpers.Random(60, 120) * 1000 : 100;

            switch (settings.BotMode)
            {
                case BotMode.FixedProfit:
                    _marketTimer.Interval = timerInterval; // Randomly every 1-2 minutes (60-120)
                    Strategy = new FixedProfitStrategy(_exchangeType, settings);
                    break;

                case BotMode.FixedPriceChange:
                    _marketTimer.Interval = timerInterval; // Randomly every 1-2 minutes (60-120)
                    Strategy = new FixedPriceChangeStrategy(_exchangeType, settings);
                    break;

                case BotMode.TradingViewSignal:
                    _marketTimer.Interval = 5000; // Every 5 seconds
                    Strategy = new TradingViewAlertStrategy(_exchangeType, settings);
                    break;

                default:
                    Console.WriteLine("Unhandled Bot Mode.");
                    Console.ReadLine();
                    return;
            }

            _init = true;
        }

        public static void Start(Settings settings)
        {
            if (!_init) Init(settings);

#if DEBUG
            Strategy.Activate();
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
            if (_exchangeType != ExchangeType.MockupExchange && string.IsNullOrEmpty(Bot.Settings.APIKey))
                throw new Exception("Settings reset!");

#if DEBUG
            try
            {
                Strategy.Activate();
            }
            catch (Exception ex)
            {
                Stop();
                Logger logger = new Logger("BacktestDataLogger.log");
                logger.WriteLine($"HydraFactor = {Settings.HydraFactor} PriceChangePerc = {Settings.PriceChangePercentage} Total Price = {Statistics.GetPortfolioValue()}");
            }
#endif

#if RELEASE
            try
            {
                NativeMethods.PreventSleep();
                Strategy.Activate();
            }
            catch (Exception ex)
            {
                Bot.WriteLog(ex.Message);
            }
#endif
        }
    }
}