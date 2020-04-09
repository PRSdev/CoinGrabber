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

        [Description("Price Change Trade")]
        PriceChangeTrade,

        [Description("Swing Trade")]
        SwingTrade
    }
}