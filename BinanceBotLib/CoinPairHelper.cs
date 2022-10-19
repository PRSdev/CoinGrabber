using ExchangeClientLib;

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
            return ExchangeClient.CoinPairsList.FindIndex(x => x.ToString() == _settings.CoinPair.ToString());
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