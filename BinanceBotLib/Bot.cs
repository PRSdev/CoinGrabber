using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public static void Start()
        {
            Random rnd = new Random();
            Timer marketTickTimer = new System.Timers.Timer();
            marketTickTimer.Interval = rnd.Next(60, 120) * 1000; // Randomly every 1-2 minutes (60-120)
            marketTickTimer.Elapsed += MarketTickTimer_Tick;
            marketTickTimer.Start();
        }

        private static void MarketTickTimer_Tick(object sender, ElapsedEventArgs e)
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