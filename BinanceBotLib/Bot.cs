using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinanceBotLib
{
    public static class Bot
    {
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

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
            logger.WriteLine(message);
        }

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
    }
}