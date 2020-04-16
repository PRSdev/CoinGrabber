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

        WebCallResult<BinancePlacedOrder> PlaceBuyOrder(TradingData trade);

        WebCallResult<BinancePlacedOrder> PlaceSellOrder(TradingData trade);

        WebCallResult<BinancePlacedOrder> PlaceTestBuyOrder(TradingData trade);

        WebCallResult<BinancePlacedOrder> PlaceTestSellOrder(TradingData trade);

        WebCallResult<BinancePlacedOrder> PlaceStopLoss(TradingData trade, decimal stopLossPerc);
    }
}