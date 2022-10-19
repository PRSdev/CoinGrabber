using Binance.Net.Enums;
using ExchangeClientLib;
using System;
using System.Collections.Generic;

namespace BinanceBotLib
{
    public class Strategy : IStrategy
    {
        public delegate void ProgressEventHandler();
        public delegate void TradingEventHandler(TradingData tradingData);

        public event ProgressEventHandler Started, Completed;
        public event TradingEventHandler TradeListItemHandled, OrderSucceeded;

        protected ExchangeClient _client = null;
        protected Settings _settings = null;
        public Statistics Statistics { get; private set; }

        public Strategy(ExchangeType exchangeType, Settings settings)
        {
            _settings = settings;

            switch (exchangeType)
            {
                case ExchangeType.BinanceExchange:
                    _client = new BinanceExchangeClient(_settings.APIKey, _settings.SecretKey);
                    break;

                case ExchangeType.MockupExchange:
                    _client = new MockupExchangeClient(_settings.APIKey, _settings.SecretKey);
                    break;
            }

            Statistics = new Statistics(settings, _client.Portfolio);
        }

        public void Activate()
        {
            OnStarted();
            Trade();
            OnCompleted();
        }

        protected TradingData GetNewTradingData()
        {
            CoinPairHelper cph = new CoinPairHelper(_settings);
            return new TradingData() { CoinPair = cph.GetCoinPair() };
        }

        public virtual void Trade()
        {
            throw new NotImplementedException();
        }

        public virtual void Trade(List<TradingData> trades)
        {
            throw new Exception("Strategy is not implemented!");
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
            if (_settings.ProductionMode)
                SettingsManager.SaveSettings(_settings);
#endif

#if RELEASE
            SettingsManager.SaveSettings(_settings);
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
            throw new NotImplementedException();
        }

        protected virtual void PlaceSellOrder(TradingData trade, bool forReal)
        {
            if (trade.CoinQuantityToTrade == 0)
            {
                throw new Exception("CoinQuantityToTrade is not set!");
            }

            decimal fees = _client.GetTradeFee(trade.CoinPair);

            decimal totalReceived = trade.CoinQuantityToTrade * trade.Price / (1 + fees);

            var sellOrder = forReal ? _client.PlaceSellOrder(trade) : _client.PlaceTestSellOrder(trade);
            if (sellOrder)
            {
                trade.SellPriceAfterFees = totalReceived / trade.CoinQuantityToTrade;
                trade.LastAction = OrderSide.Sell;
                Bot.WriteLog(trade.ToStringSold());
                if (trade.BuyPriceAfterFees > 0)
                {
                    if (forReal)
                        _settings.TotalProfit += trade.Profit;
                    else
                        _settings.TotalProfitSimulation += trade.Profit;
                }
                OnOrderSucceeded(trade);
                Sleep();
            }
        }

        public virtual void PlaceSellOrder()
        {
            throw new NotImplementedException();
        }

        protected void Sleep()
        {
            if (Bot.GetExchangeType() != ExchangeType.MockupExchange)
            {
                System.Threading.Thread.Sleep(250);
            }
        }
    }
}