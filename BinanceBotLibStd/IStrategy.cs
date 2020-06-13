using ExchangeClientLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    internal interface IStrategy
    {
        void Trade();

        void Trade(List<TradingData> tradesList);

        void PlaceBuyOrder();

        void PlaceSellOrder();
    }
}