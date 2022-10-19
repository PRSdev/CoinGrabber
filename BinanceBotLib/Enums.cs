using System.ComponentModel;

namespace BinanceBotLib
{
    public enum BotMode
    {
        [Description("Fixed Price")]
        FixedPrice
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