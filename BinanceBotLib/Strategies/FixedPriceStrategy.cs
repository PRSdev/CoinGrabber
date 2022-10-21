using ExchangeClientLib;
using System;

namespace BinanceBotLib
{
    public class FixedPriceStrategy : Strategy
    {
        public FixedPriceStrategy(ExchangeType exchange, Settings settings) : base(exchange, settings)
        {
        }

        public override void Trade()
        {
            if (_settings.CoinListingTime - DateTime.UtcNow < new TimeSpan(0, 2, 0))
            {
                PlaceBuyOrder();
            }
            else
            {
                Console.WriteLine($"Duration until listing: {_settings.CoinListingTime - DateTime.UtcNow}");
            }
        }

        public override void PlaceBuyOrder()
        {
            decimal balance = _client.GetBalance(_settings.CoinPair.Pair2);
            Bot.WriteConsole($"{_settings.CoinPair.Pair2} balance: {balance}");
            if (balance > 10)
            {
                TradingData tradingData = new TradingData();
                tradingData.CoinPair = _settings.CoinPair;
                tradingData.CoinQuantity = _client.GetMaxQuantityAfterFees(balance, _settings.CoinPair);
                if (tradingData.CoinQuantity > 0)
                {
                    Bot.WriteConsole($"Max quantity to buy: {tradingData.CoinQuantity.ToString()}");
                    _client.PlaceBuyOrder(tradingData);
                }

            }
        }

        public override void PlaceSellOrder()
        {

        }
    }
}