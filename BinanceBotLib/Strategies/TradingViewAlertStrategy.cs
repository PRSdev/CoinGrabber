using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BinanceBotLib
{
    public class TradingViewAlertStrategy : Strategy
    {
        private int _intUnreadEmails = -1;
        private OrderSide _signal;

        public TradingViewAlertStrategy(ExchangeType exchange) : base(exchange)
        {
        }

        public override void Trade()
        {
            Trade(Bot.Settings.TradingViewTradesList);
        }

        public override void Trade(List<TradingData> tradesList)
        {
            // Check for new email every second
            EmailHelper agent = new EmailHelper(Bot.Settings.GmailAddress, Bot.Settings.GmailPassword);

            // Check for stop losses
            foreach (TradingData trade in tradesList)
            {
                trade.MarketPrice = Math.Round(_client.GetPrice(trade.CoinPair));
                decimal stopLossPrice = trade.BuyPriceAfterFees * (1 - Bot.Settings.StopLossPerc / 100);
                if (trade.MarketPrice < stopLossPrice)
                {
                    PlaceSellOrder(trade, forReal: false, stopLoss: true);
                }
            }

            // Check for new email
            if (agent.UnreadEmails == _intUnreadEmails)
                return;
            else
                _intUnreadEmails = agent.UnreadEmails;

            // buy or sell signal
            if (!string.IsNullOrEmpty(agent.Advice))
            {
                if (!agent.Advice.Contains(Bot.Settings.SecretWord))
                {
                    // return; // SPAM!
                }

                // Match coin pair
                CoinPair coinPair = CoinPairs.GetCoinPair(agent.Advice);
                if (coinPair == null)
                {
                    throw new Exception("CoinPair not supported!");
                }

                if (agent.Advice.Contains("Buy"))
                {
                    _signal = OrderSide.Buy;
                }
                else if (agent.Advice.Contains("Sell"))
                {
                    _signal = OrderSide.Sell;
                }

                // cleanup
                tradesList.RemoveAll(td => td.BuyOrderID > -1 && td.SellOrderID > -1);

                // If no buy or sell orders for the required coin pair, then place an order
                TradingData tdSearch = tradesList.Find(x => x.CoinPair.Pair1 == coinPair.Pair1);

                if (tdSearch == null)
                {
                    TradingData trade = new TradingData(coinPair);
                    switch (_signal)
                    {
                        case OrderSide.Buy:
                            PlaceBuyOrder(trade, tradesList, forReal: false);
                            break;

                        case OrderSide.Sell:
                            decimal coins = _client.GetBalance(coinPair.Pair1);
                            trade.CoinQuantity = Math.Round(coins / Bot.Settings.HydraFactor, 5);
                            tradesList.Add(trade);
                            PlaceSellOrder(trade, forReal: false);
                            break;
                    }
                }
                else
                {
                    OnStarted();
                    foreach (TradingData trade in tradesList)
                    {
                        if (_signal == OrderSide.Sell)
                        {
                            PlaceSellOrder(trade, forReal: false);
                            Thread.Sleep(250);
                        }
                    }

                    if (_signal == OrderSide.Buy)
                    {
                        PlaceBuyOrder(new TradingData(coinPair), tradesList);
                    }
                }
            }
        }
    }
}