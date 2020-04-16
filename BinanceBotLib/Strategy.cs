using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public class Strategy : IStrategy
    {
        public delegate void ProgressEventHandler();
        public delegate void TradingEventHandler(TradingData tradingData);

        public event ProgressEventHandler Started, Completed;
        public event TradingEventHandler TradeListItemHandled, OrderSucceeded;

        protected static ExchangeClient _client = null;

        public Strategy(ExchangeType exchangeType)
        {
            switch (exchangeType)
            {
                case ExchangeType.BinanceExchange:
                    _client = new BinanceExchangeClient(Bot.Settings.APIKey, Bot.Settings.SecretKey);
                    break;
                case ExchangeType.SimulatedExchange:
                    _client = new MockupExchangeClient(Bot.Settings.APIKey, Bot.Settings.SecretKey);
                    break;
            }
        }

        public virtual void Trade()
        {
            throw new NotImplementedException();
        }

        public virtual void Trade(List<TradingData> trades)
        {
            throw new Exception("Strategy is not implemented!");
        }

        protected void OnStarted()
        {
            Started?.Invoke();
        }

        protected void OnTradeListItemHandled(TradingData data)
        {
            TradeListItemHandled?.Invoke(data);
        }

        protected void OnOrderSucceeded(TradingData data)
        {
            Bot.SaveSettings();
            OrderSucceeded?.Invoke(data);
        }

        protected void OnCompleted()
        {
            Completed?.Invoke();
        }

        public virtual void PlaceBuyOrder()
        {
            throw new NotImplementedException();
        }

        protected virtual void PlaceBuyOrder(TradingData trade, List<TradingData> tradesList, bool forReal = true)
        {
            decimal fiatValue = _client.GetBalance(trade.CoinPair.Pair2);

            trade.CapitalCost = fiatValue / Bot.Settings.HydraFactor;
            if (trade.CapitalCost > Bot.Settings.InvestmentMin)
            {
                trade.MarketPrice = _client.GetPrice(trade.CoinPair) * (1 - Math.Abs(Bot.Settings.BuyBelowPerc) / 100);

                Console.WriteLine();

                decimal fees = _client.GetTradeFee(trade.CoinPair);
                decimal myInvestment = trade.CapitalCost / (1 + fees);
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
            else
            {
                Console.WriteLine($"Capital cost is too low to buy more.");
            }
        }

        protected virtual void PlaceSellOrder(TradingData trade, bool forReal = true, bool stopLoss = false)
        {
            trade.MarketPrice = Math.Round(_client.GetPrice(trade.CoinPair) * (1 + Math.Abs(Bot.Settings.SellAbovePerc) / 100), 2);

            if (trade.MarketPrice > trade.BuyPriceAfterFees || stopLoss)
            {
                trade.CapitalCost = trade.CoinQuantity * trade.MarketPrice;

                if (trade.CapitalCost > Bot.Settings.InvestmentMin || stopLoss)
                {
                    decimal fees = _client.GetTradeFee(trade.CoinPair);
                    decimal myInvestment = trade.CapitalCost / (1 + fees);

                    var sellOrder = forReal ? _client.PlaceSellOrder(trade) : _client.PlaceTestSellOrder(trade);
                    if (sellOrder.Success)
                    {
                        trade.SellPriceAfterFees = myInvestment / trade.CoinQuantity;
                        trade.SellOrderID = sellOrder.Data.OrderId;
                        trade.LastAction = Binance.Net.Objects.OrderSide.Sell;
                        Bot.WriteLog(trade.ToStringSold());
                        if (trade.BuyPriceAfterFees > 0) Bot.Settings.TotalProfit += trade.Profit;
                        OnOrderSucceeded(trade);
                    }
                }
            }
        }

        public virtual void PlaceSellOrder()
        {
            throw new NotImplementedException();
        }
    }
}