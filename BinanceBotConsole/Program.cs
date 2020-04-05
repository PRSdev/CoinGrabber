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

        private static decimal _buyPrice;

        private static void Main(string[] args)
        {
            Bot.LoadSettings();

            if (Bot.Settings.APIKey == "XXX")
            {
                Console.WriteLine("Empty API Key and/or Secret Key!");
                return;
            }

            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(Bot.Settings.APIKey, Bot.Settings.SecretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            marketTickTimer.Interval = rnd.Next(6, 12) * 1000; // Randomly every 1-2 minutes (60-120)
            marketTickTimer.Elapsed += MarketTickTimer_Tick;
            marketTickTimer.Start();

            Console.ReadLine();

            Bot.SaveSettings();
        }

        private static WebCallResult<BinancePlacedOrder> _buyOrder = null;
        private static WebCallResult<BinancePlacedOrder> _sellOrder = null;
        private static WebCallResult<BinanceOrder> _queryOrder = null;

        private static void MarketTickTimer_Tick(object sender, ElapsedEventArgs e)
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                _queryOrder = client.GetOrder(CoinPairs.BTCUSDT, orderId: Bot.Settings.LastOrderID);

                if (_queryOrder.Data == null || _queryOrder.Data.Status != OrderStatus.New && Bot.Settings.DailyProfitTarget > 0)
                {
                    // We are safe to start trading
                    var coinUSDT = accountInfo.Data.Balances.Single(s => s.Asset == "USDT");

                    var price = client.GetPrice(CoinPairs.BTCUSDT);
                    var prices24h = client.Get24HPrice(CoinPairs.BTCUSDT);
                    var fees = client.GetTradeFee().Data.Single(s => s.Symbol == CoinPairs.BTCUSDT);

                    decimal entryPrice = Math.Min(Bot.Settings.InvestmentMax, coinUSDT.Free);
                    decimal myInvestment = entryPrice / (1 + fees.MakerFee);
                    Bot.WriteLine("USDT balance to trade = " + entryPrice.ToString());

                    _buyPrice = Math.Min(price.Data.Price, prices24h.Data.BidPrice);
                    Bot.Settings.CoinQuantity = Math.Round(myInvestment / _buyPrice, 6);
                    decimal receiveTarget = entryPrice + Bot.Settings.DailyProfitTarget;
                    Bot.WriteLine($"Receive target = ${receiveTarget}");

                    if (prices24h.Data.BidPrice > prices24h.Data.OpenPrice)
                    {
                        myInvestment = (entryPrice - Bot.Settings.DailyProfitTarget / 2) / (1 + fees.MakerFee);
                        _buyPrice = Math.Round(myInvestment / Bot.Settings.CoinQuantity, 2);
                        receiveTarget = entryPrice + Bot.Settings.DailyProfitTarget / 2;
                        Bot.WriteLine($"Adjusted receive target = ${receiveTarget}");
                    }

                    Bot.WriteLine($"Buying {Bot.Settings.CoinQuantity} BTC for ${_buyPrice}");
                    _buyOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Buy, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: _buyPrice, timeInForce: TimeInForce.GoodTillCancel);

                    if (_buyOrder.Success)
                    {
                        // Save Sell Price
                        Bot.Settings.SellPrice = Math.Round(receiveTarget * (1 + fees.MakerFee) / Bot.Settings.CoinQuantity, 2);
                        Bot.Settings.LastOrderID = _buyOrder.Data.OrderId;
                    }
                }
                else if (Bot.Settings.LastOrderID > 0 && Bot.Settings.SellPrice > 0 && Bot.Settings.CoinQuantity > 0)
                {
                    var queryOrder = client.GetOrder(CoinPairs.BTCUSDT, orderId: Bot.Settings.LastOrderID);

                    if (queryOrder.Data.Status == OrderStatus.Filled)
                    {
                        // We are now in BTC
                        var coinBTC = accountInfo.Data.Balances.Single(s => s.Asset == "BTC");
                        Bot.WriteLine("BTC balance to trade = " + coinBTC.Free.ToString());

                        if (coinBTC.Free > Bot.Settings.CoinQuantity)
                        {
                            Bot.WriteLine($"Selling {Bot.Settings.CoinQuantity} BTC for ${Bot.Settings.SellPrice}");
                            _sellOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Sell, OrderType.Limit, Bot.Settings.CoinQuantity, Bot.Settings.SellPrice, timeInForce: TimeInForce.GoodTillCancel);
                        }
                    }
                    else if (queryOrder.Data.Status == OrderStatus.Canceled)
                    {
                        Bot.WriteLine($"Order {Bot.Settings.LastOrderID} has been cancelled by the user.");
                        _buyOrder = null;
                    }
                    else
                    {
                        Console.WriteLine("Waiting till orders are filled...");
                    }
                }
                else
                {
                    Console.WriteLine("Unhandled outcome. Reload application...");
                }
            }
        }
    }
}