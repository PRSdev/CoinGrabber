using Binance.Net;
using ExchangeClientLib;
using System.Linq;

namespace BinanceBotLib
{
    public class FuturesRandomStrategy : Strategy
    {
        public FuturesRandomStrategy(ExchangeType exchangeType, Settings settings) : base(exchangeType, settings)
        {
        }

        public override void Trade()
        {
            using (var tempClient = new BinanceClient())
            {
                var openOrders = tempClient.FuturesUsdt.Order.GetOpenOrders().Data;
                TradingData trade = new TradingData(new CoinPair("BTC", "USDT", 3));
                var pos = tempClient.FuturesUsdt.GetPositionInformation().Data.Single(s => s.Symbol == trade.CoinPair.ToString());
                trade.UpdateMarketPrice(pos.MarkPrice);

                // If zero positions
                if (pos.EntryPrice == 0 && openOrders.Count() == 0)
                {
                    trade.CoinQuantity = 0.1m;
                    bool rLong = ShareX.HelpersLib.RandomFast.Next(1) == 1;
                    if (rLong)  // Create long position
                    {
                        // _client.PlaceBuyOrder(trade, trade.Price + 20.0m);
                        _client.PlaceBuyOrder(trade);
                        trade.UpdateMarketPrice(trade.Price + 20m);
                        _client.PlaceSellOrder(trade, closePosition: true);
                    }
                    else
                    {
                        // _client.PlaceSellOrder(trade, trade.Price - 20.0m);
                        _client.PlaceSellOrder(trade);
                        trade.UpdateMarketPrice(trade.Price - 20m);
                        _client.PlaceSellOrder(trade, closePosition: true);
                    }
                }
            }

        }
    }
}
