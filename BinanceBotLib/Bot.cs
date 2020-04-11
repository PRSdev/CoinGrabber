using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;

namespace BinanceBotLib
{
    public static class Bot
    {
        public static readonly string PersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BinanceBot");
        public static Settings Settings { get; private set; }

        public static List<CoinPair> CoinPairs = new List<CoinPair>()
        {
            new CoinPair("BTC", "USDT"),
            new CoinPair("BCH", "USDT")
        };

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

        private static void ThreadWorker_DoWork()
        {
#if DEBUG
            TradingHelper.SwingTrade();
#endif

            Random rnd = new Random();
            System.Timers.Timer marketTimer = new System.Timers.Timer();
            marketTimer.Interval = rnd.Next(60, 120) * 1000; // Randomly every 1-2 minutes (60-120)
            marketTimer.Elapsed += MarketTimer_Tick;
            marketTimer.Start();
        }

        public static void Start()
        {
            if (TradingHelper.ThreadWorker == null)
            {
                // if timer is null then Init() has not been called
                TradingHelper.Init();
                TradingHelper.ThreadWorker.DoWork += ThreadWorker_DoWork;
                TradingHelper.ThreadWorker.Completed += ThreadWorker_Completed;
            }

            TradingHelper.ThreadWorker.Start(ApartmentState.STA);
        }

        private static void ThreadWorker_Completed()
        {
            // throw new NotImplementedException();
        }

        private static void MarketTimer_Tick(object sender, ElapsedEventArgs e)
        {
            Bot.LoadSettings(); // Re-read settings

            switch (Bot.Settings.BotMode)
            {
                case BotMode.DayTrade:
                    TradingHelper.DayTrade();
                    break;
                case BotMode.SwingTrade:
                    TradingHelper.SwingTrade();
                    break;
                default:
                    Console.WriteLine("Unhandled Bot Mode.");
                    Console.ReadLine();
                    return;
            }

            Bot.SaveSettings();
        }
    }
}