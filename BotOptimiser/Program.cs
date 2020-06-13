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
            decimal fiatValue = 20000m;
            int hydraFactorMin = 10;
            int hydraFactorMax = 20;
            decimal priceChangePercMin = 1;
            decimal priceChangePercMax = 4;
            decimal priceChangePercIncr = 1;

            // BotOptimiser 20000 10 20 1.0 4.0
            if (args.Length == 6)
            {
                decimal.TryParse(args[0], out fiatValue);
                int.TryParse(args[1], out hydraFactorMin);
                int.TryParse(args[2], out hydraFactorMax);
                decimal.TryParse(args[3], out priceChangePercMin);
                decimal.TryParse(args[4], out priceChangePercMax);
                decimal.TryParse(args[5], out priceChangePercIncr);
            }

            for (int hydraFactor = hydraFactorMin; hydraFactor <= hydraFactorMax; hydraFactor++)
            {
                for (decimal priceChangePercDown = priceChangePercMin; priceChangePercDown <= priceChangePercMax; priceChangePercDown += priceChangePercIncr)
                {
                    for (decimal priceChangePercUp = priceChangePercMin; priceChangePercUp <= priceChangePercMax; priceChangePercUp += priceChangePercIncr)
                    {
                        Settings settings = new Settings()
                        {
                            CoinPair = new CoinPair("BTC", "USDT", 6),
                            HydraFactor = hydraFactor,
                            PriceChangePercentageDown = priceChangePercDown,
                            PriceChangePercentageUp = priceChangePercUp,
                            BotMode = BotMode.FixedPriceChange,
                            InvestmentMax = 0
                        };

                        Bot myBot = new Bot(settings);
                        myBot.Start(settings);
                    }
                }
            }

            Console.ReadLine();
        }
    }
}