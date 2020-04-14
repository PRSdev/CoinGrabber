using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public static class CoinPairs
    {
        public static List<CoinPair> CoinPairsList = new List<CoinPair>()
        {
            new CoinPair("BTC", "USDT"),
            new CoinPair("BCH", "USDT"),
            new CoinPair("BNB", "USDT")
        };

        public static CoinPair GetCoinPair()
        {
            return CoinPairs.CoinPairsList[GetCoinPairIndex()];
        }

        public static int GetCoinPairIndex()
        {
            if (Bot.Settings.RandomNewCoinPair)
            {
                return MathHelpers.CryptoRandom(0, CoinPairsList.Count - 1);
            }
            else
            {
                return CoinPairsList.FindIndex(x => x.ToString() == Bot.Settings.CoinPair.ToString());
            }
        }
    }
}