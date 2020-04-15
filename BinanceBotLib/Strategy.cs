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
        public event TradingEventHandler PriceChecked, OrderSucceeded;

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
            throw new Exception("Strategy is not implemented!");
        }

        protected void OnStarted()
        {
            Started?.Invoke();
        }

        protected void OnPriceChecked(TradingData data)
        {
            PriceChecked?.Invoke(data);
        }

        protected void OnOrderSucceeded(TradingData data)
        {
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

        public virtual void PlaceSellOrder()
        {
            throw new NotImplementedException();
        }
    }
}