using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BinanceBotLib
{
    public enum BotMode
    {
        [Description("Daily Trade")]
        DayTrade,

        [Description("Swing Trade")]
        SwingTrade
    }

    public enum ExchangeType
    {
        [Description("Binance")]
        BinanceExchange,

        [Description("Simulated Exchange")]
        SimulatedExchange
    }
}