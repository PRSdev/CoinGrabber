using Binance.Net;
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
                if (tempClient.GetOpenOrders().Data.Count() != 0)
                {
                    return;
                }

                TradingData trade = new TradingData(new CoinPair("BTC", "USDT", 5));
                trade.UpdateMarketPrice(_client.GetPrice(trade.CoinPair));

                // If zero orders then continue
                decimal investment = _client.GetBalance("USDT") / 11m; // 11 is to have the liquidation very low or high
                trade.CoinQuantity = investment / trade.Price * 20m; // 20x leverage

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
        }
    }
}