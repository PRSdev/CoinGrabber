using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.MarketData;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExchangeClientLib
{
    public class BinanceFuturesExchangeClient : ExchangeClient
    {
        public BinanceFuturesExchangeClient(string apiKey, string secretKey) : base(apiKey, secretKey)
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
                    decimal balance = client.FuturesUsdt.Account.GetAccountInfo().Data.TotalWalletBalance;
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
                    decimal marketPrice = Math.Round(client.FuturesUsdt.Market.GetPrice(coinPair.ToString()).Data.Price, 2);
                    Portfolio.UpdateCoinMarketPrice(coinPair.Pair1, marketPrice);
                    return marketPrice;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public async Task<WebCallResult<BinancePrice>> GetPriceAsync(CoinPair coinPair)
        {
            using (var client = new BinanceClient())
            {
                var task = await client.FuturesUsdt.Market.GetPriceAsync(coinPair.ToString());
                decimal marketPrice = Math.Round(task.Data.Price, 2);
                Portfolio.UpdateCoinMarketPrice(coinPair.Pair1, marketPrice);
                return task;
            }
        }

        public override bool PlaceBuyOrder(TradingData trade, bool closePosition)
        {
            using (var client = new BinanceClient())
            {
                var buyOrder = client.FuturesUsdt.Order.PlaceOrder(
                   trade.CoinPair.ToString(),
                   OrderSide.Buy,
                   OrderType.Limit,
                   quantity: Math.Round(trade.CoinQuantityToTrade, 3),
                   reduceOnly: closePosition,
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
                var sellOrder = client.FuturesUsdt.Order.PlaceOrder(
                   trade.CoinPair.ToString(),
                   OrderSide.Sell,
                   OrderType.Limit,
                   quantity: Math.Round(trade.CoinQuantityToTrade, 3),
                   reduceOnly: closePosition,
                   price: Math.Round(trade.Price, 2),
                   timeInForce: TimeInForce.GoodTillCancel);

                if (sellOrder.Success)
                    trade.SellOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }

        public override bool PlaceBuyOrder(TradingData trade, decimal priceTakeProfit)
        {
            using (var client = new BinanceClient())
            {
                var buyOrder = client.FuturesUsdt.Order.PlaceOrder(
                   trade.CoinPair.ToString(),
                   OrderSide.Buy,
                   OrderType.TakeProfitLimit,
                   quantity: Math.Round(trade.CoinQuantityToTrade, 3),
                   stopPrice: priceTakeProfit,
                   price: Math.Round(trade.Price, 2),
                   timeInForce: TimeInForce.GoodTillCancel);

                if (buyOrder.Success)
                    trade.BuyOrderID = buyOrder.Data.OrderId;
                else
                    Console.WriteLine(buyOrder.Error.Message.ToString());

                return buyOrder.Success;
            }
        }

        public override bool PlaceSellOrder(TradingData trade, decimal priceTakeProfit)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.FuturesUsdt.Order.PlaceOrder(
                   trade.CoinPair.ToString(),
                   OrderSide.Sell,
                   OrderType.TakeProfitLimit,
                   quantity: Math.Round(trade.CoinQuantityToTrade, 3),
                   stopPrice: priceTakeProfit,
                   price: Math.Round(trade.Price, 2),
                   timeInForce: TimeInForce.GoodTillCancel);

                if (sellOrder.Success)
                    trade.SellOrderID = sellOrder.Data.OrderId;
                else
                    Console.WriteLine(sellOrder.Error.Message.ToString());

                return sellOrder.Success;
            }
        }
    }
}