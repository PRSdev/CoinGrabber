using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public class CoinPair
    {
        public string Pair1 { get; set; }
        public string Pair2 { get; set; }

        public CoinPair()
        {
        }

        public CoinPair(string pair1, string pair2)
        {
            Pair1 = pair1;
            Pair2 = pair2;
        }

        public override string ToString()
        {
            return Pair1 + Pair2;
        }
    }
}