using BinanceBotLib;
using ExchangeClientLib;
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
                myBot.Settings.APIKey = Console.ReadLine().Trim();

                Console.Write("Enter Binance Secret Key: ");
                myBot.Settings.SecretKey = Console.ReadLine().Trim();

                SettingsManager.SaveSettings(myBot.Settings);
            }

            myBot.Settings.BotMode = BotMode.FixedPrice;

            Console.Write("Enter coin to grab: ");
            string coin = Console.ReadLine().Trim();
            myBot.Settings.CoinPair = new CoinPair(coin, "BUSD", 2);

            SettingsManager.SaveSettings(myBot.Settings);

            myBot.Start();
            Console.WriteLine($"{myBot.Settings.BotMode.GetDescription()} Bot started...");

            Console.ReadLine();
        }
    }
}