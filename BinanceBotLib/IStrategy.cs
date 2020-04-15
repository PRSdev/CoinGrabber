using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    internal interface IStrategy
    {
        void Trade();

        void PlaceBuyOrder();

        void PlaceSellOrder();
    }
}