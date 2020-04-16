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
        public static List<decimal> PriceChanges { get; set; } = new List<decimal>();

        public static decimal GetPriceChangePercAuto()
        {
            return Math.Abs(PriceChanges.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First());
        }

        public static string GetTotalProfit()
        {
            return Math.Round(Bot.Settings.TotalProfit,2).ToString();
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
                cost += trade.CapitalCost;
            }

            return cost.ToString();
        }

        public static NameValueCollection GetReport()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("Total profit made to-date ($)", GetTotalProfit());
            nvc.Add("Profit per day ($/day)", GetProfitPerDay());
            nvc.Add("Total current investment ($)", GetTotalInvestment());

            foreach (CoinData coin in ExchangeClient.Portfolio.Coins)
            {
                if (coin.Balance > 0)
                    nvc.Add($"{coin.Name} balance", coin.Balance.ToString());
            }
            return nvc;
        }
    }
}