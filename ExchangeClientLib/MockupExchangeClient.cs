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

        public MockupExchangeClient(string apiKey, string secretKey) : base(apiKey, secretKey)
        {
            _prices = File.ReadAllLines("BacktestData.csv")
                                        .Skip(1)
                                        .Select(v => HistoricalData.FromCsv(v))
                                        .ToList();
        }

        public override decimal GetBalance(string coinName)
        {
            return Portfolio.GetBalance(coinName);
        }

        public override decimal GetPrice(CoinPair coinPair)
        {
            return _prices[_currentIteration++].Open;
        }

        public override decimal GetTradeFee(CoinPair coinPair)
        {
            return 0.22m;
        }

        public override WebCallResult<BinancePlacedOrder> PlaceBuyOrder(TradingData trade)
        {
            return null;
        }
    }

    internal class HistoricalData
    {
        public DateTime Date { get; private set; }
        public decimal Open { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }
        public decimal Close { get; private set; }
        public decimal Volume { get; private set; }

        public static HistoricalData FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');

            HistoricalData data = new HistoricalData();

            data.Date = Convert.ToDateTime(values[0]);
            data.Open = Convert.ToDecimal(values[1]);
            data.High = Convert.ToDecimal(values[2]);
            data.Low = Convert.ToDecimal(values[3]);
            data.Close = Convert.ToDecimal(values[4]);
            data.Volume = Convert.ToDecimal(values[5]);

            return data;
        }
    }
}