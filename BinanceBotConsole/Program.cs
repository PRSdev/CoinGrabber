using BinanceBotLib;
using ShareX.HelpersLib;
using System;

namespace BinanceBotConsole
{
    internal class Program

    {
        private static Settings Settings { get; set; }

        private static void Main(string[] args)
        {
            Settings = Bot.LoadSettings();

            // Error handling
            if (string.IsNullOrEmpty(Settings.APIKey))
            {
                Console.Write("Enter Binance API Key: ");
                Settings.APIKey = Console.ReadLine();

                Console.Write("Enter Binance Secret Key: ");
                Settings.SecretKey = Console.ReadLine();
            }

            // Choose Bot mode
            foreach (BotMode bm in Enum.GetValues(typeof(BotMode)))
            {
                Console.WriteLine($"{bm.GetIndex()} for {bm.GetDescription()}");
            }
            Console.Write("Choose Bot mode: ");

            int intMode;
            int.TryParse(Console.ReadLine(), out intMode);
            Settings.BotMode = (BotMode)intMode;

            Bot.SaveSettings(Settings);

            // Error handling - Bot mode specific
            switch (Settings.BotMode)
            {
                case BotMode.FixedProfit:
                    if (Settings.DailyProfitTarget <= 0)
                    {
                        Console.WriteLine("Daily Profit Target must be greater than zero!");
                        Console.ReadLine();
                        return;
                    }
                    break;
            }

            Bot myBot = new Bot(Program.Settings);
            myBot.Start();
            Console.WriteLine($"{Settings.BotMode.GetDescription()} Bot started...");

            Console.ReadLine();
        }
    }
}