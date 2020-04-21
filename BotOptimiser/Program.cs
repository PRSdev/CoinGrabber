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
            for (int hydraFactor = 19; hydraFactor <= 20; hydraFactor++)
            {
                for (decimal priceChangePerc = 3.0m; priceChangePerc <= 4.0m; priceChangePerc = priceChangePerc + 0.1m)
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