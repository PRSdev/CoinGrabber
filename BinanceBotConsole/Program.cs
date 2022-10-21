using BinanceBotLib;
using ExchangeClientLib;
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

            Console.Write("Enter coin to grab (SUI): ");
            string coin = Console.ReadLine().Trim();
            myBot.Settings.CoinPair = new CoinPair(coin, "BUSD", 1); // Some coins only support one decimal

            Console.Write("Listing date and time in UTC (2022-12-25 01:00): ");
            DateTime dtListing;
            DateTime.TryParse(Console.ReadLine().Trim(), out dtListing);
            myBot.Settings.CoinListingTime = dtListing;

            SettingsManager.SaveSettings(myBot.Settings);

            myBot.Start();

            Console.ReadLine();
        }
    }
}