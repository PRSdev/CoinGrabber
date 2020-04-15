using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public class TradingViewAlertStrategy : Strategy
    {
        public TradingViewAlertStrategy(ExchangeType exchange) : base(exchange)
        {
        }

        public override void Trade()
        {
            // Check for new email every second
            EmailHelper agent = new EmailHelper(Bot.Settings.GmailAddress, Bot.Settings.GmailPassword);

            // buy or sell signal
            if (!string.IsNullOrEmpty(agent.Signal))
            {
                if (!agent.Signal.Contains(Bot.Settings.SecretWord))
                {
                    return; // SPAM!
                }

                // Match coin pair
                CoinPair coinPair = CoinPairs.GetCoinPair(agent.Signal);
                if (coinPair == null)
                {
                    throw new Exception("CoinPair not supported!");
                }

                if (agent.Signal.Contains("Buy"))
                {
                    decimal fiatValue = _client.GetBalance(coinPair.Pair2);
                    TradingData trade = new TradingData(coinPair);
                    trade.CapitalCost = Math.Round(fiatValue / Bot.Settings.HydraFactor, 2);

                    // Capital Cost is fiatValue / HydraFactor
                }
                else if (agent.Signal.Contains("Sell"))
                {
                    decimal coins = _client.GetBalance(coinPair.Pair1);
                    decimal coinsValue = coins * _client.GetPrice(coinPair);
                }
            }
        }
    }
}