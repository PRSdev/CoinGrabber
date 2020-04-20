using ExchangeClientLib;
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
                case ExchangeType.MockupExchange:
                    _client = new MockupExchangeClient(Bot.Settings.APIKey, Bot.Settings.SecretKey);
                    break;
            }
        }

        public void Activate()
        {
            OnStarted();
            Trade();
            OnCompleted();
        }

        public virtual void Trade()
        {
            throw new NotImplementedException();
        }

        public virtual void Trade(List<TradingData> trades)
        {
            throw new Exception("Strategy is not implemented!");
        }

        protected static decimal PriceChangePercentage
        {
            get
            {
                return Bot.Settings.AutoAdjustPriceChangePercentage && Statistics.PriceChanges.Count > 1000 ? Statistics.GetPriceChangePercAuto() : Math.Abs(Bot.Settings.PriceChangePercentage);
            }
        }

        #region Events

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
            OrderSucceeded?.Invoke(data);
#if DEBUG
            if (Bot.Settings.ProductionMode)
                Bot.SaveSettings();
#endif

#if RELEASE
            Bot.SaveSettings();
#endif
        }

        protected void OnCompleted()
        {
            Completed?.Invoke();
        }

        #endregion Events

        public virtual void PlaceBuyOrder()
        {
            throw new NotImplementedException();
        }

        protected virtual void PlaceBuyOrder(TradingData trade, List<TradingData> tradesList, bool forReal)
        {
            decimal fiatValue = _client.GetBalance(trade.CoinPair.Pair2);

            decimal capitalCost = fiatValue / Bot.Settings.HydraFactor;

            if (capitalCost > Bot.Settings.InvestmentMin)
            {
                trade.MarketPrice = _client.GetPrice(trade.CoinPair) * (1 - Math.Abs(Bot.Settings.BuyBelowPerc) / 100);

                Console.WriteLine();

                decimal fees = _client.GetTradeFee(trade.CoinPair);
                decimal myInvestment = capitalCost / (1 + fees);
                trade.CoinQuantity = myInvestment / trade.MarketPrice;

                var buyOrder = forReal ? _client.PlaceBuyOrder(trade) : _client.PlaceTestBuyOrder(trade);
                if (buyOrder)
                {
                    trade.BuyPriceAfterFees = capitalCost / trade.CoinQuantity;
                    trade.ID = tradesList.Count;
                    trade.LastAction = Binance.Net.Objects.OrderSide.Buy;
                    tradesList.Add(trade);
                    Bot.WriteLog(trade.ToStringBought());
                    OnOrderSucceeded(trade);
                }
            }
            else
            {
                Console.WriteLine($"Capital cost is too low to buy more.");
            }
        }

        protected virtual void PlaceSellOrder(TradingData trade, bool forReal)
        {
            decimal fees = _client.GetTradeFee(trade.CoinPair);

            decimal totalReceived = trade.CoinQuantityToTrade * trade.MarketPrice / (1 + fees);

            var sellOrder = forReal ? _client.PlaceSellOrder(trade) : _client.PlaceTestSellOrder(trade);
            if (sellOrder)
            {
                trade.SellPriceAfterFees = totalReceived / trade.CoinQuantityToTrade;
                trade.LastAction = Binance.Net.Objects.OrderSide.Sell;
                Bot.WriteLog(trade.ToStringSold());
                if (trade.BuyPriceAfterFees > 0)
                {
                    if (forReal)
                        Bot.Settings.TotalProfit += trade.Profit;
                    else
                        Bot.Settings.TotalProfitSimulation += trade.Profit;
                }
                OnOrderSucceeded(trade);
            }
        }

        public virtual void PlaceSellOrder()
        {
            throw new NotImplementedException();
        }

        protected void Sleep()
        {
            if (Bot._exchangeType != ExchangeType.MockupExchange)
            {
                System.Threading.Thread.Sleep(250);
            }
        }
    }
}