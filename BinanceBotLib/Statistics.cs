using ExchangeClientLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BinanceBotLib
{
    public static class Statistics
    {
        public static List<double> PriceChanges { get; set; } = new List<double>();

        public static decimal GetPriceChangePercAuto()
        {
            // max 3 days
            int intMax = 3 * 24 * 60;
            int intExtra = PriceChanges.Count - intMax;
            if (PriceChanges.Count > intMax) PriceChanges.RemoveRange(intExtra, PriceChanges.Count - intExtra);

            double avg = Math.Abs(PriceChanges.Average());
            double sd = Math.Abs(Math.Sqrt(PriceChanges.Average(v => Math.Pow(v - avg, 2))));

            return Math.Max(0.5m, (decimal)(avg + sd)); // Math.Abs(PriceChanges.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First());
        }

        public static string GetTotalProfit()
        {
            decimal profit = Bot.Settings.ProductionMode ? Bot.Settings.TotalProfit : Bot.Settings.TotalProfitSimulation;
            return Math.Round(profit, 2).ToString();
        }

        public static string GetProfitPerDay()
        {
            double totalDays = (DateTime.Now - Bot.Settings.StartDate).TotalDays;
            return Math.Round(Bot.Settings.TotalProfit / (decimal)totalDays, 2).ToString();
        }

        public static string GetTotalInvestment()
        {
            decimal cost = 0m;

            foreach (TradingData trade in Bot.Settings.TradingDataList)
            {
                cost += trade.CoinOriginalQuantity * trade.BuyPriceAfterFees;
            }

            return Math.Round(cost, 2).ToString();
        }

        public static string GetPortfolioValue()
        {
            decimal fiatValue = 0;
            foreach (CoinData coin in ExchangeClient.Portfolio.Coins)
            {
                if (coin.Name == "USDT")
                    fiatValue += coin.Balance;
                else if (coin.Balance > 0)
                {
                    fiatValue += coin.Value;
                }
            }

            return Math.Round(fiatValue, 2).ToString();
        }

        public static NameValueCollection GetReport()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("Total profit made to-date ($)", GetTotalProfit());
            nvc.Add("Profit per day ($/day)", GetProfitPerDay());
            nvc.Add("Total current investment ($)", GetTotalInvestment());
            nvc.Add("Portfolio value ($)", GetPortfolioValue());
            foreach (CoinData coin in ExchangeClient.Portfolio.Coins)
            {
                if (coin.Balance > 0)
                    nvc.Add($"{coin.Name} balance", coin.Balance.ToString());
            }
            return nvc;
        }
    }
}