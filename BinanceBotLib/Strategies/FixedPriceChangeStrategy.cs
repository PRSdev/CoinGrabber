using ExchangeClientLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BinanceBotLib
{
    public class FixedPriceChangeStrategy : Strategy
    {
        public FixedPriceChangeStrategy(ExchangeType exchangeType, Settings settings) : base(exchangeType, settings)
        {
        }

        public override void Trade()
        {
            Trade(_settings.TradingDataList);
        }

        public override void Trade(List<TradingData> tradesList)
        {
            // Check USDT and BTC balances
            decimal coins = _client.GetBalance(_settings.CoinPair.Pair1);
            decimal fiatValue = _client.GetBalance(_settings.CoinPair.Pair2);

            // Check if user has more USDT or more BTC
            decimal coinsValue = coins * _client.GetPrice(_settings.CoinPair);

            // cleanup
            tradesList.RemoveAll(trade => trade.BuyOrderID > -1 && trade.SellOrderID > -1);

            // If no buy or sell orders for the required coin pair, then place an order
            TradingData tdSearch = tradesList.Find(x => x.CoinPair.Pair1 == _settings.CoinPair.Pair1);
            if (tdSearch == null)
            {
                // buy or sell?
                if (fiatValue > coinsValue)
                {
                    // buy
                    PlaceBuyOrder(GetNewTradingData(), _settings.TradingDataList, _settings.ProductionMode);
                }
                else
                {
                    // sell
                    TradingData trade0 = GetNewTradingData();
                    trade0.CoinQuantity = Math.Round(coins / _settings.HydraFactor, 5);
                    tradesList.Add(trade0); // Add because this is the seed
                    PlaceSellOrder(trade0, forReal: _settings.ProductionMode);
                }
            }

            // monitor market price for price changes
            Console.WriteLine();

            foreach (TradingData trade in tradesList)
            {
                trade.MarketPrice = _client.GetPrice(trade.CoinPair);
                trade.SetPriceChangePercentage(trade.MarketPrice);
                Console.WriteLine(trade.ToStringPriceCheck());
                OnTradeListItemHandled(trade);
                // sell if positive price change
                if (trade.PriceChangePercentage > PriceChangePercentage)
                {
                    PlaceSellOrder(trade, forReal: _settings.ProductionMode);
                }
                Sleep();
            }

            TradingData lastTrade = tradesList.Last<TradingData>();
            Statistics.PriceChanges.Add((double)Math.Abs(lastTrade.PriceChangePercentage));
            Console.WriteLine($"User={_settings.PriceChangePercentage}% Bot={Statistics.GetPriceChangePercAuto()}% Interation={Statistics.PriceChanges.Count.ToString()}");
            if (lastTrade.PriceChangePercentage < PriceChangePercentage * -1)
            {
                // buy more if negative price change
                PlaceBuyOrder(GetNewTradingData(), _settings.TradingDataList, _settings.ProductionMode);
            }
        }

        public TradingData GetNewTradingData()
        {
            CoinPairHelper cph = new CoinPairHelper(_settings);
            return new TradingData() { CoinPair = cph.GetCoinPair() };
        }

        protected override void PlaceSellOrder(TradingData trade, bool forReal)
        {
            trade.MarketPrice = Math.Round(_client.GetPrice(trade.CoinPair) * (1 + Math.Abs(_settings.SellAbovePerc) / 100), 2);

            if (trade.MarketPrice > trade.BuyPriceAfterFees)
            {
                trade.CoinQuantityToTrade = trade.CoinQuantity; // trading the full amount

                decimal dmReceived = trade.CoinQuantityToTrade * trade.MarketPrice;

                if (dmReceived > _settings.InvestmentMin)
                {
                    base.PlaceSellOrder(trade, forReal);
                }
            }
        }
    }
}