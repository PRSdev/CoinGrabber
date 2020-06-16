using Binance.Net;
using ExchangeClientLib;
using System;
using System.Linq;

namespace BinanceBotLib
{
    public class FuturesStrategy : Strategy
    {
        public FuturesStrategy(ExchangeType exchangeType, Settings settings) : base(exchangeType, settings)
        {
        }

        public override void Trade()
        {
            using (var tempClient = new BinanceFuturesClient())
            {
                var openOrders = tempClient.GetOpenOrders().Data;

                TradingData trade = new TradingData(new CoinPair("BTC", "USDT", 3));
                var pos = tempClient.GetOpenPositions().Data.Single(s => s.Symbol == trade.CoinPair.ToString());
                trade.UpdateMarketPrice(pos.MarkPrice);

                Console.WriteLine($"{DateTime.Now} Entry Price: {pos.EntryPrice} Unrealised PnL: {pos.UnrealizedPnL}");

                var dataLast24hr = tempClient.Get24HPrice(trade.CoinPair.ToString()).Data;

                decimal priceDiff = dataLast24hr.WeightedAveragePrice - dataLast24hr.LowPrice;

                if (_settings.IsAutoAdjustShortAboveAndLongBelow)
                {
                    trade.PriceLongBelow = Math.Round(dataLast24hr.WeightedAveragePrice - priceDiff * 0.618m, 2);
                    decimal entryPrice = pos.EntryPrice == 0 ? dataLast24hr.WeightedAveragePrice : pos.EntryPrice;
                    trade.PriceShortAbove = Math.Round(priceDiff * 0.618m + entryPrice, 2);
                }
                else
                {
                    trade.PriceLongBelow = _settings.LongBelow;
                    trade.PriceShortAbove = _settings.ShortAbove;
                }

                if (_settings.IsAutoAdjustTargetProfit)
                {
                    trade.ProfitTarget = (double)Math.Round(Math.Abs(pos.Quantity) / pos.Leverage * pos.EntryPrice * 0.618m, 2);
                }
                else
                {
                    trade.ProfitTarget = _settings.FuturesProfitTarget;
                }

                // If zero positions
                if (pos.EntryPrice == 0 && openOrders.Count() == 0)
                {
                    decimal investment = _client.GetBalance(trade.CoinPair.Pair2) / (decimal)_settings.FuturesSafetyFactor; // 11 is to have the liquidation very low or high
                    trade.CoinQuantity = investment / trade.Price * pos.Leverage; // 20x leverage

                    // Short above or Long below
                    if (trade.Price < trade.PriceLongBelow)
                    {
                        _client.PlaceBuyOrder(trade);
                    }
                    else if (trade.Price > trade.PriceShortAbove)
                    {
                        _client.PlaceSellOrder(trade);
                    }
                }

                // When there is an existing position
                else if (pos.EntryPrice > 0)
                {
                    if (pos.LiquidationPrice < pos.EntryPrice) // Long position
                    {
                        trade.LastAction = Binance.Net.Enums.OrderSide.Buy;
                    }
                    else
                    {
                        trade.LastAction = Binance.Net.Enums.OrderSide.Sell;
                    }

                    if (pos.UnrealizedPnL > 0)
                    {
                        trade.CoinQuantity = pos.Quantity;

                        bool success;
                        bool targetProfitMet = trade.ProfitTarget > 0 && pos.UnrealizedPnL > (decimal)trade.ProfitTarget;

                        if (trade.LastAction == Binance.Net.Enums.OrderSide.Buy && (targetProfitMet || pos.MarkPrice > trade.PriceShortAbove)) // i.e. Long position
                        {
                            success = _client.PlaceSellOrder(trade, closePosition: true);
                        }
                        else if (trade.LastAction == Binance.Net.Enums.OrderSide.Sell && (targetProfitMet || pos.MarkPrice < trade.PriceLongBelow))
                        {
                            success = _client.PlaceBuyOrder(trade, closePosition: true);
                        }
                        else
                        {
                            success = false;
                        }

                        if (success)
                        {
                            _settings.TotalProfit += pos.UnrealizedPnL;
                        }
                    }
                }

                // Below is for statistics and to keep UI up-to-date
                trade.SetQuantity(pos.Quantity);
                trade.SellPriceAfterFees = trade.BuyPriceAfterFees = pos.EntryPrice;
                trade.SetPriceChangePercentage(pos.MarkPrice, isFutures: true);
                OnTradeListItemHandled(trade);
            }
        }
    }
}