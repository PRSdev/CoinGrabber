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
        FixedPriceChanage
    }

    public enum ExchangeType
    {
        [Description("Binance")]
        BinanceExchange,

        [Description("Simulated Exchange")]
        SimulatedExchange
    }
}