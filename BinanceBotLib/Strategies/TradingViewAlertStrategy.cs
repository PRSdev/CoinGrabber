using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BinanceBotLib
{
    public class TradingViewAlertStrategy : Strategy
    {
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

            if (tradesList.Count > 0)
            {
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

                // cleanup
                tradesList.RemoveAll(td => td.BuyOrderID > -1 && td.SellOrderID > -1);
            }

            // Check for new email
            if (!agent.NewMail)
                return;

            // buy or sell signal
            if (!string.IsNullOrEmpty(agent.Subject))
            {
                if (!agent.Subject.Contains(Bot.Settings.SecretWord))
                {
                    // return; // SPAM!
                }

                // Match coin pair
                CoinPair coinPair = CoinPairs.GetCoinPair(agent.Subject);
                if (coinPair == null)
                {
                    throw new Exception("CoinPair not supported!");
                }

                if (agent.Subject.Contains("Buy"))
                {
                    _signal = OrderSide.Buy;
                }
                else if (agent.Subject.Contains("Sell"))
                {
                    _signal = OrderSide.Sell;
                }

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
                else // existing trades detected
                {
                    OnStarted();

                    foreach (TradingData trade in tradesList)
                    {
                        if (_signal == OrderSide.Sell && trade.CoinQuantity > trade.CoinOriginalQuantity * Bot.Settings.SellMaxQuantityPerc / 100)
                        {
                            trade.CoinQuantity = Math.Round(trade.CoinQuantity * Bot.Settings.SellQuantityPerc / 100, 5);
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