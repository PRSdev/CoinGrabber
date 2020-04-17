using Binance.Net.Objects;
using ExchangeClientLib;
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
            OnStarted();

            if (tradesList.Count > 0)
            {
                // Check for stop losses
                foreach (TradingData trade in tradesList)
                {
                    OnTradeListItemHandled(trade);
                    trade.MarketPrice = _client.GetPrice(trade.CoinPair);
                    if (trade.MarketPrice > 0)
                    {
                        // Update PriceChangePercentage
                        if (trade.BuyPriceAfterFees > 0)
                            trade.PriceChangePercentage = (trade.MarketPrice - trade.BuyPriceAfterFees) / trade.BuyPriceAfterFees * 100;
                        else if (trade.SellPriceAfterFees > 0)
                            trade.PriceChangePercentage = (trade.SellPriceAfterFees - trade.MarketPrice) / trade.SellPriceAfterFees * 100;

                        decimal stopLossPrice = trade.BuyPriceAfterFees * (1 - Bot.Settings.StopLossPerc / 100);
                        if (trade.MarketPrice < stopLossPrice)
                        {
                            PlaceSellOrder(trade, forReal: false);
                        }
                    }
                }

                // cleanup
                tradesList.RemoveAll(td => td.BuyOrderID > -1 && td.SellOrderID > -1);
            }

            // Check for new email every second
            EmailHelper agent = new EmailHelper(Bot.Settings.GmailAddress, Bot.Settings.GmailPassword);

            // Check for new email
            if (!agent.NewMail)
            {
                Console.WriteLine("No new mail!");
                return;
            }

            // buy or sell signal
            if (!string.IsNullOrEmpty(agent.Subject))
            {
                if (!agent.Subject.Contains(Bot.Settings.SecretWord))
                {
                    return; // SPAM!
                }

                // Match coin pair
                CoinPair coinPair = CoinPairHelper.GetCoinPair(agent.Subject);
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
                    foreach (TradingData trade in tradesList)
                    {
                        if (_signal == OrderSide.Sell && trade.CoinQuantity > trade.CoinOriginalQuantity * Bot.Settings.SellMaxQuantityPerc / 100)
                        {
                            trade.CoinQuantity = trade.CoinQuantity * Bot.Settings.SellQuantityPerc / 100;
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

        protected override void PlaceSellOrder(TradingData trade, bool forReal)
        {
            trade.MarketPrice = Math.Round(_client.GetPrice(trade.CoinPair), 2);
            trade.CapitalCost = trade.CoinQuantity * trade.MarketPrice;
            base.PlaceSellOrder(trade, forReal);
        }

        protected override void PlaceBuyOrder(TradingData trade, List<TradingData> tradesList, bool forReal = true)
        {
            decimal fiatValue = _client.GetBalance(trade.CoinPair.Pair2);

            if (trade.CapitalCost == 0)
                trade.CapitalCost = fiatValue / Bot.Settings.HydraFactor;

            trade.MarketPrice = _client.GetPrice(trade.CoinPair);

            Console.WriteLine();

            decimal fees = _client.GetTradeFee(trade.CoinPair);
            decimal myInvestment = trade.CapitalCost / (1 + fees);

            if (trade.CoinQuantity == 0)
                trade.CoinQuantity = myInvestment / trade.MarketPrice;

            var buyOrder = forReal ? _client.PlaceBuyOrder(trade) : _client.PlaceTestBuyOrder(trade);
            if (buyOrder.Success)
            {
                trade.BuyPriceAfterFees = trade.CapitalCost / trade.CoinQuantity;
                trade.BuyOrderID = buyOrder.Data.OrderId;
                trade.ID = tradesList.Count;
                trade.LastAction = Binance.Net.Objects.OrderSide.Buy;
                tradesList.Add(trade);
                Bot.WriteLog(trade.ToStringBought());
                OnOrderSucceeded(trade);
            }
            else
            {
                Bot.WriteLog(buyOrder.Error.Message.ToString());
            }
        }
    }
}