using ExchangeClientLib;
using System;
using System.Collections.Specialized;
using System.Text;

namespace BinanceBotLib
{
    public class Statistics
    {
        private Settings _settings;
        private PortfolioHelper _portfolio;

        public Statistics(Settings settings, PortfolioHelper portfolio)
        {
            _settings = settings;
            _portfolio = portfolio;
        }

        public string GetTotalProfit()
        {
            decimal profit = _settings.ProductionMode ? _settings.TotalProfit : _settings.TotalProfitSimulation;
            return Math.Round(profit, 2).ToString();
        }

        public string GetProfitPerDay()
        {
            double totalDays = (DateTime.Now - _settings.StartDate).TotalDays;
            return Math.Round(_settings.TotalProfit / (decimal)totalDays, 2).ToString();
        }

        public string GetPortfolioValue()
        {
            decimal fiatValue = 0;
            foreach (CoinData coin in _portfolio.Coins)
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

        public NameValueCollection GetReport()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("Total profit made to-date ($)", GetTotalProfit());
            nvc.Add("Profit per day ($/day)", GetProfitPerDay());
            nvc.Add("Portfolio value ($)", GetPortfolioValue());
            foreach (CoinData coin in _portfolio.Coins)
            {
                if (coin.Balance > 0)
                    nvc.Add($"{coin.Name} balance", coin.Balance.ToString());
            }
            nvc.Add("Last activity", DateTime.Now.ToString("g"));
            return nvc;
        }

        public string GetCoinsBalanceCsv()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CoinData coin in _portfolio.Coins)
            {
                if (coin.Balance > 0)
                    sb.Append($",{coin.Name},{coin.Balance.ToString()}");
            }

            return sb.ToString();
        }
    }
}