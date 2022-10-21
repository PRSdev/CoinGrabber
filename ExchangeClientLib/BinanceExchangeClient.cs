using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using System;
using System.Linq;

namespace ExchangeClientLib
{
    public class BinanceExchangeClient : ExchangeClient
    {
        BinanceClient _client;

        public BinanceExchangeClient(string apiKey, string secretKey) : base(apiKey, secretKey)
        {
            _client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(apiKey, secretKey),
                SpotApiOptions = new BinanceApiClientOptions
                {
                    BaseAddress = BinanceApiAddresses.Default.RestClientAddress,
                    AutoTimestamp = false
                },
            });
        }

        public override decimal GetBalance(string coinName)
        {
            try
            {
                decimal balance = _client.SpotApi.Account.GetAccountInfoAsync().Result.Data.Balances.Single(s => s.Asset == coinName).Available;
                Portfolio.UpdateCoinBalance(coinName, balance);
                return balance;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public override decimal GetPrice(CoinPair coinPair)
        {
            try
            {
                decimal marketPrice = _client.SpotApi.ExchangeData.GetPriceAsync(coinPair.ToString()).Result.Data.Price;
                Portfolio.UpdateCoinMarketPrice(coinPair.Pair1, marketPrice);
                Console.WriteLine($"{coinPair.ToString()} price: {marketPrice}");
                return marketPrice;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns Trade fee in a percentage e.g. 0.1 for 0.1% fee
        /// </summary>
        /// <param name="coinPair"></param>
        /// <returns>Returns Trade fee in a percentage e.g. 0.1 for 0.1% fee</returns>
        public override decimal GetTradeFee(CoinPair coinPair)
        {
            return _client.SpotApi.ExchangeData.GetTradeFeeAsync().Result.Data.Single(s => s.Symbol == coinPair.ToString()).TakerFee * 100;
        }

        public override decimal GetMaxQuantityAfterFees(decimal balance, CoinPair coinPair)
        {
            try
            {
                decimal price = GetPrice(coinPair);
                return Math.Floor(balance / price * 10) / 10;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public override bool PlaceBuyOrder(TradingData trade, bool closePosition = false)
        {
            var buyOrder = _client.SpotApi.Trading.PlaceOrderAsync(
            trade.CoinPair.ToString(),
            OrderSide.Buy,
            SpotOrderType.Market,
            quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision));

            if (buyOrder.Result.Success)
            {
                trade.BuyOrderID = buyOrder.Result.Data.Id;
                Console.WriteLine($"Success: {trade.BuyOrderID}");
                Console.WriteLine();
            }
            else
                Console.WriteLine(buyOrder.Result.Error.Message.ToString());

            return buyOrder.Result.Success;
        }

        public override bool PlaceSellOrder(TradingData trade, bool closePosition = false)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.SpotApi.Trading.PlaceOrderAsync(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                SpotOrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, 2),
                price: Math.Round(trade.Price, 2),
                timeInForce: TimeInForce.GoodTillCanceled);

                if (sellOrder.Result.Success)
                    trade.SellOrderID = sellOrder.Result.Data.Id;
                else
                    Console.WriteLine(sellOrder.Result.Error.Message.ToString());

                return sellOrder.Result.Success;
            }
        }

        public override bool PlaceTestBuyOrder(TradingData trade)
        {
            var buyOrder = _client.SpotApi.Trading.PlaceTestOrderAsync(
            trade.CoinPair.ToString(),
            OrderSide.Buy,
            SpotOrderType.Market,
            quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision));

            if (buyOrder.Result.Success)
            {
                trade.BuyOrderID = buyOrder.Result.Data.Id;
                Console.WriteLine($"Success: {trade.BuyOrderID}");
                Console.WriteLine();
            }

            else
                Console.WriteLine(buyOrder.Result.Error.Message.ToString());

            return buyOrder.Result.Success;
        }

        public override bool PlaceTestSellOrder(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.SpotApi.Trading.PlaceTestOrderAsync(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                SpotOrderType.Limit,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.Price, 2),
                timeInForce: TimeInForce.GoodTillCanceled);

                if (sellOrder.Result.Success)
                    trade.SellOrderID = sellOrder.Result.Data.Id;
                else
                    Console.WriteLine(sellOrder.Result.Error.Message.ToString());

                return sellOrder.Result.Success;
            }
        }

        public override bool PlaceStopLoss(TradingData trade, decimal percStopLoss)
        {
            using (var client = new BinanceClient())
            {
                var sellOrder = client.SpotApi.Trading.PlaceTestOrderAsync(
                trade.CoinPair.ToString(),
                OrderSide.Sell,
                SpotOrderType.StopLoss,
                quantity: Math.Round(trade.CoinQuantityToTrade, trade.CoinPair.Precision),
                price: Math.Round(trade.Price, 2),
                timeInForce: TimeInForce.GoodTillCanceled,
                stopPrice: Math.Round(trade.BuyPriceAfterFees * (1 - percStopLoss / 100), 2));

                if (sellOrder.Result.Success)
                    trade.SellOrderID = sellOrder.Result.Data.Id;
                else
                    Console.WriteLine(sellOrder.Result.Error.Message.ToString());

                return sellOrder.Result.Success;
            }
        }
    }
}