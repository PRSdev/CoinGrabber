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

            // Choose Bot mode
            foreach (BotMode bm in Enum.GetValues(typeof(BotMode)))
            {
                Console.WriteLine($"{bm.GetIndex()} for {bm.GetDescription()}");
            }
            Console.Write("Choose Bot mode: ");

            int intMode;
            int.TryParse(Console.ReadLine(), out intMode);
            myBot.Settings.BotMode = (BotMode)intMode;

            Bot.SaveSettings(myBot.Settings);

            // Error handling - Bot mode specific
            switch (myBot.Settings.BotMode)
            {
                case BotMode.FixedProfit:
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