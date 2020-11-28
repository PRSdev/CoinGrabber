using Binance.Net;
using ExchangeClientLib;
using ShareX.HelpersLib;
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
                trade.UpdatePrice(pos.MarkPrice);

                // If zero positions
                if (pos.EntryPrice == 0 && openOrders.Count() == 0)
                {
                    trade.CoinQuantity = 0.5m;
                    RandomFast.Run(() =>
                    {
                        _client.PlaceBuyOrder(trade);
                    }, () =>
                    {
                        _client.PlaceSellOrder(trade);
                    });
                }
                // When there is an existing position
                else if (pos.EntryPrice > 0 && openOrders.Count() == 0)
                {
                    trade.CoinQuantity = pos.Quantity;

                    if (pos.LiquidationPrice < pos.EntryPrice) // Long position
                    {
                        trade.LastAction = Binance.Net.Enums.OrderSide.Buy;
                        trade.UpdatePrice(pos.EntryPrice + 10m);
                        _client.PlaceSellOrder(trade, closePosition: true);
                    }
                    else
                    {
                        trade.LastAction = Binance.Net.Enums.OrderSide.Sell;
                        trade.UpdatePrice(pos.EntryPrice - 10m);
                        _client.PlaceBuyOrder(trade, closePosition: true);
                    }
                }

                OnTradeListItemHandled(trade);
            }

        }
    }
}
