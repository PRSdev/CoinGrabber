using Binance.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeClientLib
{
    public interface IExchangeClient
    {
        decimal GetBalance(string coin);

        decimal GetPrice(CoinPair coinPair);

        decimal GetTradeFee(CoinPair coinPair);

        bool PlaceBuyOrder(TradingData trade, bool closePosition = false);

        bool PlaceSellOrder(TradingData trade, bool closePosition = false);

        bool PlaceTestBuyOrder(TradingData trade);

        bool PlaceTestSellOrder(TradingData trade);

        bool PlaceStopLoss(TradingData trade, decimal stopLossPerc);
    }
}