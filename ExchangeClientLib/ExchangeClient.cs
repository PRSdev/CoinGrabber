using Binance.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeClientLib
{
    public class ExchangeClient : IExchangeClient
    {
        public static List<CoinPair> CoinPairsList = new List<CoinPair>()
        {
            new CoinPair("BNB", "USDT", 2),
            new CoinPair("BTC", "USDT", 6),
            new CoinPair("BCH", "USDT", 5)
        };

        public PortfolioHelper Portfolio { get; protected set; } = new PortfolioHelper();

        public ExchangeClient(string apiKey, string secretKey)
        {
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("API Key or Secret Key is empty!");
            }
        }

        public virtual decimal GetBalance(string coinName)
        {
            throw new NotImplementedException();
        }

        public virtual decimal GetPrice(CoinPair coinPair)
        {
            throw new NotImplementedException();
        }

        public virtual decimal GetTradeFee(CoinPair coinPair)
        {
            throw new NotImplementedException();
        }

        public virtual bool PlaceBuyOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual bool PlaceSellOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual bool PlaceTestBuyOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual bool PlaceTestSellOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual bool PlaceStopLoss(TradingData trade, decimal stopLossPerc)
        {
            throw new NotImplementedException();
        }
    }
}