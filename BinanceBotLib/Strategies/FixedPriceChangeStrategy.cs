using ExchangeClientLib;
using System;
using System.Collections.Generic;
using System.Linq;

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
                if (fiatValue > _settings.InvestmentMin)
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
            Bot.WriteConsole();

            foreach (TradingData trade in tradesList)
            {
                if (trade.UpdatePrice(_client.GetPrice(trade.CoinPair)))
                {
                    trade.SetPriceChangePercentage(trade.Price);
                    Bot.WriteConsole(trade.ToStringPriceCheck());
                    OnTradeListItemHandled(trade);
                    // sell if positive price change
                    if (trade.PriceChangePercentage > Math.Abs(_settings.PriceChangePercentageUp))
                    {
                        PlaceSellOrder(trade, forReal: _settings.ProductionMode);
                    }
                    Sleep();
                }
            }

            TradingData lastTrade = tradesList.Last<TradingData>();

            if (lastTrade.PriceChangePercentage < Math.Abs(_settings.PriceChangePercentageDown) * -1)
            {
                // buy more if negative price change
                PlaceBuyOrder(GetNewTradingData(), _settings.TradingDataList, _settings.ProductionMode);
            }
        }

        protected override void PlaceSellOrder(TradingData trade, bool forReal)
        {
            if (trade.UpdatePrice(Math.Round(_client.GetPrice(trade.CoinPair), 2)))
            {
                if (trade.Price > trade.BuyPriceAfterFees)
                {
                    trade.CoinQuantityToTrade = trade.CoinQuantity; // trading the full amount

                    decimal dmReceived = trade.CoinQuantityToTrade * trade.Price;

                    if (dmReceived > _settings.InvestmentMin)
                    {
                        base.PlaceSellOrder(trade, forReal);
                    }
                }
            }
        }
    }
}