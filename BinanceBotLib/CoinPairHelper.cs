using ExchangeClientLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public static class CoinPairHelper
    {
        public static CoinPair GetCoinPair()
        {
            return ExchangeClient.CoinPairsList[GetCoinPairIndex()];
        }

        public static int GetCoinPairIndex()
        {
            if (Bot.Settings.RandomNewCoinPair)
            {
                return MathHelpers.CryptoRandom(0, ExchangeClient.CoinPairsList.Count - 1);
            }
            else
            {
                return ExchangeClient.CoinPairsList.FindIndex(x => x.ToString() == Bot.Settings.CoinPair.ToString());
            }
        }

        public static CoinPair GetCoinPair(string signal)
        {
            foreach (CoinPair cp in ExchangeClient.CoinPairsList)
            {
                if (signal.Contains(cp.ToString()))
                    return cp;
            }

            return null;
        }
    }
}