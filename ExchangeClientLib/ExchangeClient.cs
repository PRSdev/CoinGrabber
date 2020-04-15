using Binance.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public class ExchangeClient : IExchangeClient
    {
        public ExchangeClient(string apiKey, string secretKey)
        {
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("API Key or Secret Key is empty!");
            }
        }

        public virtual decimal GetBalance(string coin)
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

        public virtual WebCallResult<BinancePlacedOrder> PlaceBuyOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual WebCallResult<BinancePlacedOrder> PlaceSellOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual WebCallResult<BinancePlacedOrder> PlaceTestBuyOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual WebCallResult<BinancePlacedOrder> PlaceTestSellOrder(TradingData trade)
        {
            throw new NotImplementedException();
        }

        public virtual WebCallResult<BinancePlacedOrder> PlaceStopLoss(TradingData trade, decimal stopLossPerc)
        {
            throw new NotImplementedException();
        }
    }
}