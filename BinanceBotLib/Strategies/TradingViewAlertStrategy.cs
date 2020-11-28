using Binance.Net.Enums;
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

        public TradingViewAlertStrategy(ExchangeType exchange, Settings settings) : base(exchange, settings)
        {
        }

        public override void Trade()
        {
            Trade(_settings.TradingViewTradesList);
        }

        public override void Trade(List<TradingData> tradesList)
        {
            if (tradesList.Count > 0)
            {
                // Check for stop losses
                foreach (TradingData trade in tradesList)
                {
                    OnTradeListItemHandled(trade);
                    if (trade.UpdatePrice(_client.GetPrice(trade.CoinPair)))
                    {
                        // Update PriceChangePercentage
                        trade.SetPriceChangePercentage(trade.Price);

                        decimal stopLossPrice = trade.BuyPriceAfterFees * (1 - _settings.StopLossPerc / 100);
                        if (trade.Price < stopLossPrice)
                        {
                            PlaceCompleteSellOrder(trade, forReal: _settings.ProductionMode);
                        }
                    }
                }

                // cleanup
                tradesList.RemoveAll(td => td.BuyOrderID > -1 && td.SellOrderID > -1 && td.CoinQuantity == 0);
            }

            // Check for new email every second
            EmailHelper agent = new EmailHelper(_settings);

            // Check for new email
            if (!agent.NewMail)
            {
                Bot.WriteConsole($"{DateTime.Now} No new mail!");
                return;
            }

            // buy or sell signal
            if (!string.IsNullOrEmpty(agent.Subject))
            {
                if (!agent.Subject.Contains(_settings.SecretWord))
                {
                    return; // SPAM!
                }

                // Match coin pair
                CoinPair coinPair = new CoinPairHelper(_settings).GetCoinPair(agent.Subject);
                if (coinPair == null)
                {
                    Console.WriteLine("CoinPair not supported!");
                    return;
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
                            PlaceBuyOrder(trade, tradesList, forReal: _settings.ProductionMode);
                            break;
                    }
                }
                else // existing trades detected
                {
                    foreach (TradingData trade in tradesList)
                    {
                        if (_signal == OrderSide.Sell)
                        {
                            if (_settings.SellAllOnSellSignal)
                                PlaceCompleteSellOrder(trade, _settings.ProductionMode);
                            else
                                PlacePartialSellOrder(trade, forReal: _settings.ProductionMode);
                        }
                    }
                }
            }
        }

        protected void PlaceCompleteSellOrder(TradingData trade, bool forReal)
        {
            if (trade.UpdatePrice(Math.Round(_client.GetPrice(trade.CoinPair), 2)))
            {
                trade.CoinQuantityToTrade = trade.CoinQuantity;
                base.PlaceSellOrder(trade, forReal);
            }
        }

        private void PlacePartialSellOrder(TradingData trade, bool forReal)
        {
            if (trade.CoinQuantity > trade.CoinOriginalQuantity * _settings.SellMaxQuantityPerc / 100)
            {
                trade.CoinQuantityToTrade = trade.CoinQuantity * _settings.SellQuantityPerc / 100;
                base.PlaceSellOrder(trade, forReal);
            }
        }

        protected override void PlaceBuyOrder(TradingData trade, List<TradingData> tradesList, bool forReal)
        {
            decimal fiatValue = _client.GetBalance(trade.CoinPair.Pair2);

            decimal capitalCost = fiatValue / _settings.HydraFactor;

            if (trade.UpdatePrice(_client.GetPrice(trade.CoinPair)))
            {
                Bot.WriteConsole();

                decimal fees = _client.GetTradeFee(trade.CoinPair);
                decimal myInvestment = capitalCost / (1 + fees);

                if (trade.CoinQuantity == 0)
                    trade.CoinQuantity = myInvestment / trade.Price;

                var buyOrder = forReal ? _client.PlaceBuyOrder(trade) : _client.PlaceTestBuyOrder(trade);
                if (buyOrder)
                {
                    trade.BuyPriceAfterFees = capitalCost / trade.CoinQuantity;
                    trade.ID = tradesList.Count;
                    trade.LastAction = OrderSide.Buy;
                    tradesList.Add(trade);
                    Bot.WriteLog(trade.ToStringBought());
                    OnOrderSucceeded(trade);
                }
            }
        }
    }
}