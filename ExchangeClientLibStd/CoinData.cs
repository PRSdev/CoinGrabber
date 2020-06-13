using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeClientLib
{
    public class CoinData
    {
        public string Name { get; set; }

        public decimal Balance { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal Value
        {
            get
            {
                return Math.Round(Balance * MarketPrice, 2);
            }
        }

        public CoinData(string name)
        {
            Name = name;
        }
    }
}