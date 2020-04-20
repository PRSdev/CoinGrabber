using ExchangeClientLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public class CoinPairHelper
    {
        private Settings _settings;

        public CoinPairHelper(Settings settings)
        {
            _settings = settings;
        }

        public CoinPair GetCoinPair()
        {
            return ExchangeClient.CoinPairsList[GetCoinPairIndex()];
        }

        public int GetCoinPairIndex()
        {
            if (_settings.RandomNewCoinPair)
            {
                return MathHelpers.CryptoRandom(0, ExchangeClient.CoinPairsList.Count - 1);
            }
            else
            {
                return ExchangeClient.CoinPairsList.FindIndex(x => x.ToString() == _settings.CoinPair.ToString());
            }
        }

        public CoinPair GetCoinPair(string signal)
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