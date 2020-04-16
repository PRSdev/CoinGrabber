using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeClientLib
{
    public class CoinData
    {
        public string Name { get; set; }

        public decimal Quantity { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal Value
        {
            get
            {
                return Math.Round(Quantity * MarketPrice, 2);
            }
        }
    }
}