using Binance.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public class ExchangeClient : IExchangeClient
    {
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
    }
}