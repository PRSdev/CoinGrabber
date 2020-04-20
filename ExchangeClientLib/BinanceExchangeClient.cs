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

namespace ExchangeClientLib
{
    public class BinanceExchangeClient : ExchangeClient
    {
        public BinanceExchangeClient(string apiKey, string secretKey) : base(apiKey, secretKey)
        {
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(apiKey, secretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });
        }

        public override decimal GetBalance(string coinName)
        {
            using (var client = new BinanceClient())
            {
                decimal balance = client.GetAccountInfo().Data.Balances.Single(s => s.Asset == coinName).Free;
                Portfolio.UpdateCoinBalance(coinName, balance);
                return balance;
            }
        }

        public override decimal GetPrice(CoinPair coinPair)
        {
            using (var client = new BinanceClient())
            {
                decimal marketPrice = Math.Round(client.GetPrice(coinPair.ToString()).Data.Price, 2);
                Portfolio.UpdateCoinMarketPrice(coinPair.Pair1, marketPrice);
                return marketPrice;
            }
        }

        public override decimal GetTradeFee(CoinPair coinPair)
        {
            using (var client = new BinanceClient())
            {
                return client.GetTradeFee().Data.Single(s => s.Symbol == coinPair.ToString()).MakerFee;
            }
        }

        public override bool PlaceBuyOrder(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                var buyOrder = client.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Buy,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.MarketPrice, 2),
                timeInForce: TimeInForce.GoodTillCancel);

                if (buyOrder.Success)
                    trade.BuyOrderID = buyOrder.Data.OrderId;
                else
                    Console.WriteLine(buyOrder.Error.Message.ToString());

                return buyOrder.Success;
            }
        }

        public override bool PlaceSellOrder(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.MarketPrice, 2),
                timeInForce: TimeInForce.GoodTillCancel);

                if (sellOrder.Success)
                    trade.BuyOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }

        public override bool PlaceTestBuyOrder(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                var buyOrder = client.PlaceTestOrder(
                trade.CoinPair.ToString(),
                OrderSide.Buy,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.MarketPrice, 2),
                timeInForce: TimeInForce.GoodTillCancel);

                if (buyOrder.Success)
                    trade.BuyOrderID = buyOrder.Data.OrderId;
                else
                    Console.WriteLine(buyOrder.Error.Message.ToString());

                return buyOrder.Success;
            }
        }

        public override bool PlaceTestSellOrder(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.PlaceTestOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.MarketPrice, 2),
                timeInForce: TimeInForce.GoodTillCancel);

                if (sellOrder.Success)
                    trade.BuyOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }

        public override bool PlaceStopLoss(TradingData trade, decimal percStopLoss)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.PlaceTestOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.StopLoss,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.MarketPrice, 2),
                timeInForce: TimeInForce.GoodTillCancel,
                stopPrice: Math.Round(trade.BuyPriceAfterFees * (1 - percStopLoss / 100), 2));

                if (sellOrder.Success)
                    trade.BuyOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }
    }
}