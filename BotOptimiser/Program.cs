using BinanceBotLib;
using ExchangeClientLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotOptimiser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            for (int hydraFactor = 10; hydraFactor <= 20; hydraFactor++)
            {
                for (decimal priceChangePerc = 1.0m; priceChangePerc <= 4.0m; priceChangePerc++)
                {
                    Settings settings = new Settings()
                    {
                        CoinPair = new CoinPair("BTC", "USDT", 6),
                        HydraFactor = hydraFactor,
                        PriceChangePercentage = priceChangePerc,
                        BotMode = BotMode.FixedPriceChange,
                        InvestmentMax = 0
                    };

                    Bot myBot = new Bot(settings);
                    myBot.Start();
                }
            }

            Console.ReadLine();
        }
    }
}