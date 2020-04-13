using Binance.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public interface IExchangeClient
    {
        decimal GetBalance(string coin);

        decimal GetPrice(CoinPair coinPair);

        decimal GetTradeFee(CoinPair coinPair);

        bool PlaceBuyOrder(TradingData trade);

        bool PlaceSellOrder(TradingData trade);
    }
}