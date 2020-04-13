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

        public BinanceExchangeClient(string apiKey, string secretKey)
        {
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(apiKey, secretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            client = new BinanceClient();
        }

        public override decimal GetBalance(string coin)
        {
            return client.GetAccountInfo().Data.Balances.Single(s => s.Asset == coin).Free;
        }

        public override decimal GetPrice(CoinPair coinPair)
        {
            return Math.Round(client.GetPrice(coinPair.ToString()).Data.Price, 2);
        }

        public override decimal GetTradeFee(CoinPair coinPair)
        {
            return client.GetTradeFee().Data.Single(s => s.Symbol == coinPair.ToString()).MakerFee;
        }

        public override WebCallResult<BinancePlacedOrder> PlaceBuyOrder(TradingData trade)
        {
            var buyOrder = client.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Buy,
                OrderType.Limit,
                quantity: trade.CoinQuantity,
                price: Math.Round(trade.MarketPrice, 2),
                timeInForce: TimeInForce.GoodTillCancel);

            return buyOrder;
        }

        public override WebCallResult<BinancePlacedOrder> PlaceSellOrder(TradingData trade)
        {
            var sellOrder = client.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.Limit,
                quantity: trade.CoinQuantity,
                price: Math.Round(trade.MarketPrice, 2),
                timeInForce: TimeInForce.GoodTillCancel);

            return sellOrder;
        }
    }
}