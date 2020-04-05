using Binance.Net;
using Binance.Net.Objects;
using BinanceBotLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CryptoExchange.Net.Objects;

namespace BinanceBotConsole
{
    internal class Program
    {
        private static Timer marketTickTimer = new Timer();
        private static Random rnd = new Random();

        private static void Main(string[] args)
        {
            Bot.LoadSettings();

            // Error handling
            if (string.IsNullOrEmpty(Bot.Settings.APIKey))
            {
                Console.Write("Enter Binance API Key: ");
                Bot.Settings.APIKey = Console.ReadLine();

                Console.Write("Enter Binance Secret Key: ");
                Bot.Settings.SecretKey = Console.ReadLine();
            }

            if (Bot.Settings.DailyProfitTarget <= 0)
            {
                Console.WriteLine("Daily Profit Target must be greater than zero!");
                Console.ReadLine();
                return;
            }

            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(Bot.Settings.APIKey, Bot.Settings.SecretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            marketTickTimer.Interval = rnd.Next(60, 120) * 1000; // Randomly every 1-2 minutes (60-120)
            marketTickTimer.Elapsed += MarketTickTimer_Tick;
            marketTickTimer.Start();
            Console.WriteLine("Bot initiated...");

            Console.ReadLine();

            Bot.SaveSettings();
        }

        private static void MarketTickTimer_Tick(object sender, ElapsedEventArgs e)
        {
            using (var client = new BinanceClient())
            {
                var queryOrder = client.GetOrder(CoinPairs.BTCUSDT, orderId: Bot.Settings.LastOrderID);

                if (queryOrder.Data != null)
                {
                    switch (queryOrder.Data.Status)
                    {
                        case OrderStatus.Filled:
                            SellOrder();
                            break;
                        case OrderStatus.Canceled:
                            Bot.WriteLine($"Order {Bot.Settings.LastOrderID} has been cancelled by the user.");
                            BuyOrder();
                            break;
                        case OrderStatus.New:
                            Console.WriteLine($"Waiting for the order {Bot.Settings.LastOrderID} to fill...");
                            break;
                        default:
                            Console.WriteLine("Unhandled outcome. Reload application...");
                            break;
                    }
                }
                else if (queryOrder.Data == null)
                {
                    BuyOrder();
                }
            }
        }

        private static void BuyOrder()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                var coinUSDT = accountInfo.Data.Balances.Single(s => s.Asset == "USDT");

                var price = client.GetPrice(CoinPairs.BTCUSDT);
                var prices24h = client.Get24HPrice(CoinPairs.BTCUSDT);
                var fees = client.GetTradeFee().Data.Single(s => s.Symbol == CoinPairs.BTCUSDT);

                decimal entryPrice = Math.Min(Bot.Settings.InvestmentMax, coinUSDT.Free);
                decimal myInvestment = entryPrice / (1 + fees.MakerFee);
                Bot.WriteLine("USDT balance to trade = " + entryPrice.ToString());

                decimal marketPrice = Math.Min(price.Data.Price, prices24h.Data.BidPrice);
                decimal myBuyPrice = marketPrice;
                Bot.WriteLine("Market price = $" + marketPrice);

                Bot.Settings.CoinQuantity = Math.Round(myInvestment / myBuyPrice, 6);
                decimal receiveTarget = entryPrice + Bot.Settings.DailyProfitTarget;
                Bot.WriteLine($"Receive target = ${receiveTarget}");

                if (prices24h.Data.BidPrice > prices24h.Data.OpenPrice)
                {
                    myInvestment = (entryPrice - Bot.Settings.DailyProfitTarget / 2) / (1 + fees.MakerFee);
                    myBuyPrice = Math.Round(myInvestment / Bot.Settings.CoinQuantity, 2);
                    receiveTarget = entryPrice + Bot.Settings.DailyProfitTarget / 2;
                    Bot.WriteLine($"Adjusted receive target = ${receiveTarget}");
                }

                Bot.WriteLine($"Buying {Bot.Settings.CoinQuantity} BTC for ${myBuyPrice}");
                var buyOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Buy, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: myBuyPrice, timeInForce: TimeInForce.GoodTillCancel);

                if (buyOrder.Success)
                {
                    // Save Sell Price
                    Bot.Settings.SellPrice = Math.Round(receiveTarget * (1 + fees.MakerFee) / Bot.Settings.CoinQuantity, 6);
                    Bot.WriteLine("Target sell price = $" + Bot.Settings.SellPrice);

                    decimal priceDiff = Bot.Settings.SellPrice - myBuyPrice;
                    decimal priceChange = Math.Round(priceDiff / myBuyPrice * 100, 2);
                    Console.WriteLine($"Price change = {priceChange}%");
                    Bot.Settings.LastOrderID = buyOrder.Data.OrderId;
                    Console.WriteLine();
                }
            }
        }

        private static void SellOrder()
        {
            if (Bot.Settings.SellPrice > 0 && Bot.Settings.CoinQuantity > 0)
            {
                using (var client = new BinanceClient())
                {
                    var accountInfo = client.GetAccountInfo();

                    var coinBTC = accountInfo.Data.Balances.Single(s => s.Asset == "BTC");
                    Bot.WriteLine("BTC balance to trade = " + coinBTC.Free.ToString());

                    if (coinBTC.Free > Bot.Settings.CoinQuantity)
                    {
                        Bot.WriteLine($"Selling {Bot.Settings.CoinQuantity} BTC for ${Bot.Settings.SellPrice}");
                        var sellOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Sell, OrderType.Limit, Bot.Settings.CoinQuantity, Bot.Settings.SellPrice, timeInForce: TimeInForce.GoodTillCancel);
                    }
                }
            }
        }
    }
}