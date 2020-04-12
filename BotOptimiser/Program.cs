using BinanceBotLib;
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
            decimal[] marketPrices = { 7000m, 7100m, 7200m };

            for (decimal hydraFactor = 1.0m; hydraFactor <= 5.0m; hydraFactor++)
            {
                for (decimal priceChangePerc = 1.0m; priceChangePerc <= 4.0m; priceChangePerc++)
                {
                    foreach (decimal marketPrice in marketPrices)
                    {
                        Bot.SwingTrade(hydraFactor, priceChangePerc, marketPrice);
                    }

                    Console.WriteLine($"HydraFactor = {hydraFactor} PriceChangePerc = {priceChangePerc} Total Price = {Bot.Settings.TotalProfit.ToString()}");
                }
            }
        }
    }
}