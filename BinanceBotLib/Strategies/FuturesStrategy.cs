using Binance.Net;
using Binance.Net.Objects.Futures.FuturesData;
using ExchangeClientLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                decimal priceDiff = dataLast24hr.HighPrice - dataLast24hr.LowPrice;
                if (_settings.IsAutoLongBelow)
                    trade.PriceLongOpen = Math.Round(dataLast24hr.HighPrice - priceDiff * 0.618m, 2);
                else
                    trade.PriceLongOpen = _settings.LongBelow;

                trade.PriceShortOpen = Math.Round(priceDiff * 1.618m + pos.EntryPrice, 2);

                Console.WriteLine($"PriceLongBelow: {trade.PriceLongOpen} PriceShortAbove: {trade.PriceShortOpen}");

                if (pos.EntryPrice == 0 && openOrders.Count() == 0)
                {
                    // If zero orders then continue
                    decimal investment = _client.GetBalance(trade.CoinPair.Pair2) / _settings.FuturesSafetyFactor; // 11 is to have the liquidation very low or high
                    trade.CoinQuantity = investment / trade.Price * pos.Leverage; // 20x leverage

                    // Short above or Long below
                    if (trade.Price < trade.PriceLongOpen)
                    {
                        _client.PlaceBuyOrder(trade);
                    }
                    else if (trade.Price > _settings.ShortAbove)
                    {
                        _client.PlaceSellOrder(trade);
                    }
                }
                else
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

                        if (trade.LastAction == Binance.Net.Enums.OrderSide.Buy && pos.MarkPrice > _settings.ShortAbove) // i.e. Long position
                        {
                            success = _client.PlaceSellOrder(trade, closePosition: true);
                        }
                        else if (trade.LastAction == Binance.Net.Enums.OrderSide.Sell && pos.MarkPrice < _settings.LongBelow)
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