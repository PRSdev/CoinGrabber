using System.ComponentModel;

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
        Futures,

        [Description("Futures Random")]
        FuturesRandom
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