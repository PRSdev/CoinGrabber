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
                TradingData trade = new TradingData(new CoinPair("BTC", "USDT", 3));
                trade.UpdateMarketPrice(_client.GetPrice(trade.CoinPair));

                var pos = tempClient.GetOpenPositions().Data.Single(s => s.Symbol == trade.CoinPair.ToString());

                Console.WriteLine($"Unrealised PnL: {pos.UnrealizedPnL}");

                if (pos.EntryPrice == 0)
                {
                    // If zero orders then continue
                    decimal investment = _client.GetBalance(trade.CoinPair.Pair2) / 110m; // 11 is to have the liquidation very low or high
                    trade.CoinQuantity = investment / trade.Price * pos.Leverage; // 20x leverage

                    // Short above or Long below
                    if (trade.Price < _settings.LongBelow)
                    {
                        _client.PlaceBuyOrder(trade);
                    }
                    else if (trade.Price > _settings.ShortAbove)
                    {
                        _client.PlaceSellOrder(trade);
                    }
                }
                else if (pos.UnrealizedPnL > _settings.TargetUnrealizedPnL)
                {
                   
                    trade.CoinQuantity = pos.Quantity;

                    if (pos.LiquidationPrice < pos.EntryPrice) // Long position
                    {
                        _client.PlaceSellOrder(trade, closePosition: true);
                    }
                    else
                    {
                        _client.PlaceBuyOrder(trade, closePosition: true);
                    }
                }
            }
        }
    }
}