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

                Bot.SaveSettings(); // Save API Key and Secret Key
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
                var queryBuyOrder = client.GetOrder(CoinPairs.BTCUSDT, orderId: Bot.Settings.LastBuyOrderID);
                var querySellOrder = client.GetOrder(CoinPairs.BTCUSDT, orderId: Bot.Settings.LastSellOrderID);

                if (queryBuyOrder.Data != null)
                {
                    switch (queryBuyOrder.Data.Status)
                    {
                        case OrderStatus.Filled:
                            SellOrder();
                            break;
                        case OrderStatus.Canceled:
                            Bot.WriteLog($"Buy order {Bot.Settings.LastBuyOrderID} has been cancelled by the user.");
                            BuyOrder();
                            break;
                        case OrderStatus.New:
                            Console.WriteLine($"Waiting for the buy order {Bot.Settings.LastBuyOrderID} to fill...");
                            break;
                        default:
                            Console.WriteLine("Unhandled buy order outcome. Reload application...");
                            break;
                    }
                }
                else if (querySellOrder.Data != null)
                {
                    switch (querySellOrder.Data.Status)
                    {
                        case OrderStatus.Filled:
                            BuyOrder();
                            break;
                        case OrderStatus.Canceled:
                            Bot.WriteLog($"Sell order {Bot.Settings.LastSellOrderID} has been cancelled by the user.");
                            SellOrder();
                            break;
                        case OrderStatus.New:
                            Console.WriteLine($"Waiting for the sell order {Bot.Settings.LastSellOrderID} to fill...");
                            break;
                        default:
                            Console.WriteLine("Unhandled sell order outcome. Reload application...");
                            break;
                    }
                }
                else if (querySellOrder.Data == null)
                {
                    Console.WriteLine("Could not find any previous buy orders.");
                    SellOrder();
                }
                else if (queryBuyOrder.Data == null)
                {
                    Console.WriteLine("Could not find any previous buy orders.");
                    BuyOrder();
                }
            }

            Bot.LoadSettings(); // Re-read settings
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
                Bot.WriteLog("USDT balance to trade = " + entryPrice.ToString());

                decimal marketPrice = Math.Min(price.Data.Price, prices24h.Data.BidPrice);
                decimal myBuyPrice = marketPrice;
                Bot.WriteLog("Market price = $" + marketPrice);

                Bot.Settings.CoinQuantity = Math.Round(myInvestment / myBuyPrice, 6);
                decimal receiveTarget = entryPrice + Bot.Settings.DailyProfitTarget;
                Bot.WriteLog($"Receive target = ${receiveTarget}");

                if (prices24h.Data.BidPrice > prices24h.Data.OpenPrice)
                {
                    myInvestment = (entryPrice - Bot.Settings.DailyProfitTarget / 2) / (1 + fees.MakerFee);
                    myBuyPrice = Math.Round(myInvestment / Bot.Settings.CoinQuantity, 2);
                    receiveTarget = entryPrice + Bot.Settings.DailyProfitTarget / 2;
                    Bot.WriteLog($"Adjusted receive target = ${receiveTarget}");
                }

                Bot.WriteLog($"Buying {Bot.Settings.CoinQuantity} BTC for ${myBuyPrice}");
                var buyOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Buy, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: myBuyPrice, timeInForce: TimeInForce.GoodTillCancel);

                if (buyOrder.Success)
                {
                    // Save Sell Price
                    Bot.Settings.SellPrice = Math.Round(receiveTarget * (1 + fees.MakerFee) / Bot.Settings.CoinQuantity, 2);
                    Bot.WriteLog("Target sell price = $" + Bot.Settings.SellPrice);

                    decimal priceDiff = Bot.Settings.SellPrice - myBuyPrice;
                    decimal priceChange = Math.Round(priceDiff / myBuyPrice * 100, 2);
                    Console.WriteLine($"Price change = {priceChange}%");
                    Bot.Settings.LastBuyOrderID = buyOrder.Data.OrderId;
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
                    Bot.WriteLog("BTC balance to trade = " + Bot.Settings.CoinQuantity.ToString());

                    if (coinBTC.Free > Bot.Settings.CoinQuantity)
                    {
                        Bot.WriteLog($"Selling {Bot.Settings.CoinQuantity} BTC for ${Bot.Settings.SellPrice}");
                        var sellOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Sell, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: Bot.Settings.SellPrice, timeInForce: TimeInForce.GoodTillCancel);

                        if (sellOrder.Success)
                        {
                            Bot.Settings.LastSellOrderID = sellOrder.Data.OrderId;
                            Bot.SaveSettings();
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
    }
}