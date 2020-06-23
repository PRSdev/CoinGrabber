using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BinanceBotLib
{
    public enum BotMode
    {
        [Description("Fixed Profit")]
        FixedProfit,

        [Description("Fixed Price Change")]
        FixedPriceChange,

        [Description("TradingView Signal")]
        TradingViewSignal,

        [Description("Futures")]
        Futures
    }

    public enum FuturesTakeProfitMode
    {
        ProfitByTarget,
        ProfitByPriceRange,
        ProfitByAny
    }

    public enum ExchangeType
    {
        [Description("Binance")]
        BinanceExchange,

        [Description("Binance Futures")]
        BinanceFuturesExchange,

        [Description("Mockup Exchange")]
        MockupExchange
    }
}