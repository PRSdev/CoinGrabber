using Binance.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExchangeClientLib
{
    public class MockupExchangeClient : ExchangeClient
    {
        private List<HistoricalData> _prices;
        private int _currentIteration;

        public MockupExchangeClient(string apiKey, string secretKey) : base(apiKey = "apiKey", secretKey = "secretKey")
        {
            _prices = File.ReadAllLines("BacktestData.csv")
                                        .Skip(1)
                                        .Select(v => HistoricalData.FromCsv(v))
                                        .Reverse()
                                        .ToList();

            Portfolio.UpdateCoinBalance("USDT", 3000m);
        }

        public override decimal GetBalance(string coinName)
        {
            return Portfolio.GetBalance(coinName);
        }

        public override decimal GetPrice(CoinPair coinPair)
        {
            decimal marketPrice = _prices[_currentIteration++].Open;
            Portfolio.UpdateCoinMarketPrice(coinPair.Pair1, marketPrice);
            return marketPrice;
        }

        public override decimal GetTradeFee(CoinPair coinPair)
        {
            return 0.001m;
        }

        public override bool PlaceBuyOrder(TradingData trade)
        {
            return PlaceTestBuyOrder(trade);
        }

        public override bool PlaceSellOrder(TradingData trade)
        {
            return PlaceTestBuyOrder(trade);
        }

        public override bool PlaceTestBuyOrder(TradingData trade)
        {
            Portfolio.UpdateCoinBalance(trade.CoinPair.Pair1, Portfolio.GetBalance(trade.CoinPair.Pair1) + trade.CoinQuantityToTrade);
            Portfolio.UpdateCoinBalance(trade.CoinPair.Pair2, Portfolio.GetBalance(trade.CoinPair.Pair2) - trade.CoinQuantityToTrade * trade.MarketPrice * (1 + GetTradeFee(trade.CoinPair)));
            trade.BuyOrderID = 0;
            return true;
        }

        public override bool PlaceTestSellOrder(TradingData trade)
        {
            Portfolio.UpdateCoinBalance(trade.CoinPair.Pair1, Portfolio.GetBalance(trade.CoinPair.Pair1) - trade.CoinQuantityToTrade);
            Portfolio.UpdateCoinBalance(trade.CoinPair.Pair2, Portfolio.GetBalance(trade.CoinPair.Pair2) + trade.CoinQuantityToTrade * trade.MarketPrice * (1 + GetTradeFee(trade.CoinPair)));
            trade.SellOrderID = 0;
            return true;
        }
    }

    internal class HistoricalData
    {
        public string Date { get; private set; }
        public decimal Open { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }
        public decimal Close { get; private set; }

        public static HistoricalData FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');

            HistoricalData data = new HistoricalData();

            data.Date = values[0];
            data.Open = Convert.ToDecimal(values[2]);
            data.High = Convert.ToDecimal(values[3]);
            data.Low = Convert.ToDecimal(values[4]);
            data.Close = Convert.ToDecimal(values[5]);

            return data;
        }
    }
}