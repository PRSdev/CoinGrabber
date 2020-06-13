using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeClientLib
{
    public class CoinPair
    {
        public string Pair1 { get; set; }
        public string Pair2 { get; set; }

        [JsonIgnore]
        private int _precision { get; set; }

        public CoinPair(string pair1, string pair2, int precision)
        {
            Pair1 = pair1;
            Pair2 = pair2;
            _precision = precision;
        }

        public override string ToString()
        {
            return Pair1 + Pair2;
        }

        public int Precision
        {
            get
            {
                return ExchangeClient.CoinPairsList.Find(x => x.Pair1 == Pair1)._precision;
            }
        }
    }
}