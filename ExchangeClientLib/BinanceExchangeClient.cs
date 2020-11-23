using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                try
                {
                    decimal balance = client.General.GetAccountInfo().Data.Balances.Single(s => s.Asset == coinName).Free;
                    Portfolio.UpdateCoinBalance(coinName, balance);
                    return balance;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public override decimal GetPrice(CoinPair coinPair)
        {
            using (var client = new BinanceClient())
            {
                try
                {
                    decimal marketPrice = Math.Round(client.Spot.Market.GetPrice(coinPair.ToString()).Data.Price, 2);
                    Portfolio.UpdateCoinMarketPrice(coinPair.Pair1, marketPrice);
                    return marketPrice;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public override decimal GetTradeFee(CoinPair coinPair)
        {
            using (var client = new BinanceClient())
            {
                return client.Spot.Market.GetTradeFee().Data.Single(s => s.Symbol == coinPair.ToString()).MakerFee;
            }
        }

        public override bool PlaceBuyOrder(TradingData trade, bool closePosition = false)
        {
            using (var client = new BinanceClient())
            {
                var buyOrder = client.Spot.Order.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Buy,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.Price, 2),
                timeInForce: TimeInForce.GoodTillCancel);

                if (buyOrder.Success)
                    trade.BuyOrderID = buyOrder.Data.OrderId;
                else
                    Console.WriteLine(buyOrder.Error.Message.ToString());

                return buyOrder.Success;
            }
        }

        public override bool PlaceSellOrder(TradingData trade, bool closePosition = false)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.Spot.Order.PlaceOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.Price, 2),
                timeInForce: TimeInForce.GoodTillCancel);

                if (sellOrder.Success)
                    trade.SellOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }

        public override bool PlaceTestBuyOrder(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                var buyOrder = client.Spot.Order.PlaceTestOrder(
                trade.CoinPair.ToString(),
                OrderSide.Buy,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.Price, 2),
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
                var sellOrder = client.Spot.Order.PlaceTestOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.Price, 2),
                timeInForce: TimeInForce.GoodTillCancel);

                if (sellOrder.Success)
                    trade.SellOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }

        public override bool PlaceStopLoss(TradingData trade, decimal percStopLoss)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.Spot.Order.PlaceTestOrder(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                OrderType.StopLoss,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.Price, 2),
                timeInForce: TimeInForce.GoodTillCancel,
                stopPrice: Math.Round(trade.BuyPriceAfterFees * (1 - percStopLoss / 100), 2));

                if (sellOrder.Success)
                    trade.SellOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }
    }
}