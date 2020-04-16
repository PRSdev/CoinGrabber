using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeClientLib
{
    public class PortfolioHelper
    {
        public List<CoinData> Coins = new List<CoinData>();

        public PortfolioHelper()
        {
            Coins.Add(new CoinData("USDT"));

            foreach (CoinPair cp in ExchangeClient.CoinPairsList)
            {
                Coins.Add(new CoinData(cp.Pair1));
            }
        }

        public void UpdateCoinBalance(string coinName, decimal balance)
        {
            CoinData coin = Coins.Find(x => x.Name == coinName);
            if (coin != null)
            {
                coin.Balance = balance;
            }
        }
    }
}