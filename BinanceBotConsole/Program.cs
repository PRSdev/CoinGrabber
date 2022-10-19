using BinanceBotLib;
using ShareX.HelpersLib;
using System;

namespace BinanceBotConsole
{
    internal class Program

    {
        private static Bot myBot = new Bot();

        private static void Main(string[] args)
        {
            // Error handling
            if (string.IsNullOrEmpty(myBot.Settings.APIKey))
            {
                Console.Write("Enter Binance API Key: ");
                myBot.Settings.APIKey = Console.ReadLine();

                Console.Write("Enter Binance Secret Key: ");
                myBot.Settings.SecretKey = Console.ReadLine();
            }

            myBot.Settings.BotMode = BotMode.FixedPrice;

            SettingsManager.SaveSettings(myBot.Settings);

            // Error handling - Bot mode specific
            switch (myBot.Settings.BotMode)
            {
                case BotMode.FixedPrice:
                    if (myBot.Settings.DailyProfitTarget <= 0)
                    {
                        Console.WriteLine("Daily Profit Target must be greater than zero!");
                        Console.ReadLine();
                        return;
                    }
                    break;
            }

            myBot.Start();
            Console.WriteLine($"{myBot.Settings.BotMode.GetDescription()} Bot started...");

            Console.ReadLine();
        }
    }
}