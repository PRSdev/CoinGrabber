using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BinanceBotLib
{
    public class BinanceExchangeClient : ExchangeClient
    {
        private BinanceClient client = null;

        public BinanceExchangeClient()
        {
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(Bot.Settings.APIKey, Bot.Settings.SecretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            client = new BinanceClient();
        }

        public override decimal GetBalance(string coin)
        {
            return base.GetBalance(coin);
        }

        public override decimal GetPrice(CoinPair coinPair)
        {
            return client.GetPrice(coinPair.ToString()).Data.Price;
        }

        public override decimal GetTradeFee(CoinPair coinPair)
        {
            return client.GetTradeFee().Data.Single(s => s.Symbol == coinPair.ToString()).MakerFee;
        }

        public override bool PlaceBuyOrder(TradingData trade)
        {
            var buyOrder = client.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Buy,
                OrderType.Limit,
                quantity: trade.CoinQuantity,
                price: trade.MarketPrice * (1 - Math.Abs(Bot.Settings.BuyBelowPerc) / 100),
                timeInForce: TimeInForce.GoodTillCancel);

            return buyOrder.Success;
        }

        public override bool PlaceSellOrder(TradingData trade)
        {
            var sellOrder = client.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.Limit,
                quantity: trade.CoinQuantity,
                price: trade.MarketPrice * (1 + Math.Abs(Bot.Settings.SellAbovePerc) / 100),
                timeInForce: TimeInForce.GoodTillCancel);

            return sellOrder.Success;
        }
    }
}