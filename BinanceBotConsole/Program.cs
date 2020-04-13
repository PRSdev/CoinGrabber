using BinanceBotLib;
using ShareX.HelpersLib;
using System;

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

            Bot.SaveSettings();

            // Error handling - Bot mode specific
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

            Bot.Start();
            Console.WriteLine($"{Bot.Settings.BotMode.GetDescription()} Bot started...");

            Console.ReadLine();
        }
    }
}