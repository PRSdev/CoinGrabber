using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BinanceBotLib
{
    public static class TradingHelper
    {
        public delegate void ProgressEventHandler();
        public delegate void TradingEventHandler(TradingData tradingData);

        public static event ProgressEventHandler Started;
        public static event TradingEventHandler PriceChecked, OrderSucceeded;

        internal static ThreadWorker ThreadWorker { get; private set; }

        public static void Init()
        {
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(Bot.Settings.APIKey, Bot.Settings.SecretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            if (ThreadWorker == null)
            {
                ThreadWorker = new ThreadWorker();
            }
        }

        public static void DayTrade()
        {
            using (var client = new BinanceClient())
            {
                var queryBuyOrder = client.GetOrder(Bot.Settings.CoinPair.ToString(), orderId: Bot.Settings.LastBuyOrderID); ;
                var querySellOrder = client.GetOrder(Bot.Settings.CoinPair.ToString(), orderId: Bot.Settings.LastSellOrderID);

                if (queryBuyOrder.Data != null)
                {
                    switch (queryBuyOrder.Data.Status)
                    {
                        case OrderStatus.Filled:
                            TradingHelper.SellOrderDayTrade();
                            break;
                        case OrderStatus.Canceled:
                            Bot.WriteLog($"Buy order {Bot.Settings.LastBuyOrderID} has been cancelled by the user.");
                            TradingHelper.BuyOrderDayTrade();
                            break;
                        case OrderStatus.New:
                            Console.WriteLine($"Waiting {DateTime.UtcNow - queryBuyOrder.Data.Time} for the {Bot.Settings.BuyPrice} buy order to fill...");
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
                            TradingHelper.BuyOrderDayTrade();
                            break;
                        case OrderStatus.Canceled:
                            Bot.WriteLog($"Sell order {Bot.Settings.LastSellOrderID} has been cancelled by the user.");
                            TradingHelper.SellOrderDayTrade();
                            break;
                        case OrderStatus.New:
                            Console.WriteLine($"Waiting {DateTime.UtcNow - querySellOrder.Data.Time} for the {Bot.Settings.SellPrice} sell order to fill...");
                            break;
                        default:
                            Console.WriteLine("Unhandled sell order outcome. Reload application...");
                            break;
                    }
                }
                else if (queryBuyOrder.Data == null)
                {
                    Console.WriteLine("Could not find any previous buy orders.");
                    TradingHelper.BuyOrderDayTrade();
                }
                else if (querySellOrder.Data == null)
                {
                    Console.WriteLine("Could not find any previous sell orders.");
                    TradingHelper.SellOrderDayTrade();
                }
            }
        }

        private static void BuyOrderDayTrade()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                decimal coinsUSDT = accountInfo.Data.Balances.Single(s => s.Asset == Bot.Settings.CoinPair.Pair2).Free;

                decimal myCapitalCost = Bot.Settings.InvestmentMax == 0 ? coinsUSDT : Math.Min(Bot.Settings.InvestmentMax, coinsUSDT);
                Bot.WriteLog("USDT balance to trade = " + myCapitalCost.ToString());

                decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == Bot.Settings.CoinPair.ToString()).MakerFee;
                decimal myInvestment = myCapitalCost / (1 + fees);

                decimal myRevenue = myCapitalCost + Bot.Settings.DailyProfitTarget;
                Bot.WriteLog($"Receive target = {myRevenue}");

                // New method from: https://docs.google.com/spreadsheets/d/1be6zYuzKyJMZ4Yn_pUmIt-YRTON3YJKbq_lenL2Kldc/edit?usp=sharing
                decimal marketBuyPrice = client.GetPrice(Bot.Settings.CoinPair.ToString()).Data.Price;
                Bot.WriteLog($"Market price = {marketBuyPrice}");

                decimal marketSellPrice = myRevenue * (1 + fees) / (myInvestment / marketBuyPrice);
                decimal priceDiff = marketSellPrice - marketBuyPrice;

                Bot.Settings.BuyPrice = Math.Round(marketBuyPrice - priceDiff / 2, 2);
                Bot.Settings.CoinQuantity = Math.Round(myInvestment / Bot.Settings.BuyPrice, 6);

                Bot.WriteLog($"Buying {Bot.Settings.CoinQuantity} {Bot.Settings.CoinPair.Pair1} for {Bot.Settings.BuyPrice}");
                var buyOrder = client.PlaceOrder(Bot.Settings.CoinPair.ToString(), OrderSide.Buy, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: Bot.Settings.BuyPrice, timeInForce: TimeInForce.GoodTillCancel);

                if (buyOrder.Success)
                {
                    // Save Sell Price
                    Bot.Settings.SellPrice = Math.Round(myRevenue * (1 + fees) / Bot.Settings.CoinQuantity, 2);
                    Bot.WriteLog("Target sell price = " + Bot.Settings.SellPrice);

                    decimal priceChange = Math.Round(priceDiff / Bot.Settings.BuyPrice * 100, 2);
                    Console.WriteLine($"Price change = {priceChange}%");
                    Bot.Settings.LastBuyOrderID = buyOrder.Data.OrderId;
                    Bot.WriteLog("Order ID: " + buyOrder.Data.OrderId);
                    Bot.SaveSettings();
                    Console.WriteLine();
                }
            }
        }

        private static void SellOrderDayTrade()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                decimal coinsQuantity = accountInfo.Data.Balances.Single(s => s.Asset == Bot.Settings.CoinPair.Pair1).Free;

                // if user has crypto rather than USDT for capital, then calculate SellPrice and CoinQuanitity
                if (Bot.Settings.SellPrice == 0 || Bot.Settings.CoinQuantity == 0)
                {
                    decimal marketBuyPrice = client.GetPrice(Bot.Settings.CoinPair.ToString()).Data.Price;
                    decimal myCapitalCost = Bot.Settings.InvestmentMax == 0 ? marketBuyPrice * coinsQuantity : Math.Min(Bot.Settings.InvestmentMax, marketBuyPrice * coinsQuantity);

                    decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == Bot.Settings.CoinPair.ToString()).MakerFee;
                    decimal myInvestment = myCapitalCost / (1 + fees);

                    decimal myRevenue = myCapitalCost + Bot.Settings.DailyProfitTarget;

                    decimal marketSellPrice = myRevenue * (1 + fees) / (myInvestment / marketBuyPrice);
                    decimal priceDiff = marketSellPrice - marketBuyPrice;

                    Bot.Settings.BuyPrice = Math.Round(marketBuyPrice - priceDiff / 2, 2);
                    Bot.Settings.CoinQuantity = Math.Round(myInvestment / Bot.Settings.BuyPrice, 6);
                    Bot.Settings.SellPrice = Math.Round(myRevenue * (1 + fees) / Bot.Settings.CoinQuantity, 2);
                }

                if (Bot.Settings.SellPrice > 0 && Bot.Settings.CoinQuantity > 0 && coinsQuantity > Bot.Settings.CoinQuantity)
                {
                    var sellOrder = client.PlaceOrder(Bot.Settings.CoinPair.ToString(), OrderSide.Sell, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: Bot.Settings.SellPrice, timeInForce: TimeInForce.GoodTillCancel);

                    if (sellOrder.Success)
                    {
                        Bot.WriteLog($"Sold {Bot.Settings.CoinQuantity} {Bot.Settings.CoinPair.Pair1} for {Bot.Settings.SellPrice}");
                        Bot.Settings.LastSellOrderID = sellOrder.Data.OrderId;
                        Bot.WriteLog("Order ID: " + sellOrder.Data.OrderId);
                        Bot.Settings.TotalProfit += Bot.Settings.DailyProfitTarget;
                        Bot.SaveSettings();
                        Console.WriteLine();
                    }
                }
            }
        }

        public static void SwingTrade()
        {
            // Check USDT and BTC balances
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                decimal coins = accountInfo.Data.Balances.Single(s => s.Asset == Bot.Settings.CoinPair.Pair1).Free;
                decimal fiatValue = accountInfo.Data.Balances.Single(s => s.Asset == Bot.Settings.CoinPair.Pair2).Free;

                // Check if user has more USDT or more BTC
                decimal coinsValue = coins * client.GetPrice(Bot.Settings.CoinPair.ToString()).Data.Price;

                // cleanup
                Bot.Settings.TradingDataList.RemoveAll(trade => trade.BuyOrderID > -1 && trade.SellOrderID > -1);

                // If no buy or sell orders for the required coin pair, then place an order
                TradingData tdSearch = Bot.Settings.TradingDataList.Find(x => x.CoinPair.Pair1 == Bot.Settings.CoinPair.Pair1);
                if (tdSearch == null)
                {
                    // buy or sell?
                    if (fiatValue > coinsValue)
                    {
                        // buy
                        BuyOrderSwingTrade();
                    }
                    else
                    {
                        // sell
                        TradingData trade0 = TradingData.GetNew();
                        trade0.CoinQuantity = Math.Round(coins / Bot.Settings.HydraFactor, 5);
                        Bot.Settings.TradingDataList.Add(trade0);
                        SellOrderSwingTrade(trade0);
                    }
                }
                else
                {
                    // monitor market price for price changes
                    Console.WriteLine();
                    OnStarted();
                    foreach (TradingData trade in Bot.Settings.TradingDataList)
                    {
                        trade.MarketPrice = Math.Round(client.GetPrice(trade.CoinPair.ToString()).Data.Price, 2);
                        trade.PriceChangePercentage = Math.Round((trade.MarketPrice - trade.BuyPriceAfterFees) / trade.BuyPriceAfterFees * 100, 2);
                        Console.WriteLine(trade.ToStringPriceCheck());
                        OnPriceChecked(trade);
                        // sell if positive price change
                        if (trade.PriceChangePercentage > Bot.Settings.PriceChangePercentage)
                        {
                            SellOrderSwingTrade(trade);
                        }
                        Thread.Sleep(200);
                    }

                    if (Bot.Settings.TradingDataList.Last<TradingData>().PriceChangePercentage < Bot.Settings.PriceChangePercentage * -1)
                    {
                        // buy more if negative price change
                        BuyOrderSwingTrade();
                    }
                }
            }
        }

        private static void BuyOrderSwingTrade()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                TradingData trade = TradingData.GetNew();
                decimal coinsUSDT = accountInfo.Data.Balances.Single(s => s.Asset == trade.CoinPair.Pair2).Free;

                trade.CapitalCost = Math.Round(coinsUSDT / Bot.Settings.HydraFactor, 2);
                if (trade.CapitalCost > Bot.Settings.InvestmentMin)
                {
                    trade.MarketPrice = client.GetPrice(trade.CoinPair.ToString()).Data.Price;
                    if (trade.MarketPrice < Bot.Settings.BuyBelow)
                    {
                        Console.WriteLine();

                        decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == trade.CoinPair.ToString()).MakerFee;
                        decimal myInvestment = trade.CapitalCost / (1 + fees);
                        trade.CoinQuantity = Math.Round(myInvestment / trade.MarketPrice, 5);

                        var buyOrder = client.PlaceOrder(trade.CoinPair.ToString(), OrderSide.Buy, OrderType.Limit, quantity: trade.CoinQuantity, price: trade.MarketPrice, timeInForce: TimeInForce.GoodTillCancel);
                        if (buyOrder.Success)
                        {
                            trade.BuyPriceAfterFees = Math.Round(trade.CapitalCost / trade.CoinQuantity, 2);
                            trade.BuyOrderID = buyOrder.Data.OrderId;
                            trade.ID = Bot.Settings.TradingDataList.Count;
                            Bot.Settings.TradingDataList.Add(trade);
                            Bot.WriteLog(trade.ToStringBought());
                            OnOrderSucceeded(trade);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Market price is above maximum allowable buy Price.");
                    }
                }
                else
                {
                    Console.WriteLine($"Capital cost is too low to buy more.");
                }
            }
        }

        private static void SellOrderSwingTrade(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                trade.MarketPrice = client.GetPrice(trade.CoinPair.ToString()).Data.Price;

                if (trade.MarketPrice > Bot.Settings.SellAbove)
                {
                    trade.CapitalCost = trade.CoinQuantity * trade.MarketPrice;

                    if (trade.CapitalCost > Bot.Settings.InvestmentMin)
                    {
                        decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == trade.CoinPair.ToString()).MakerFee;
                        decimal myInvestment = trade.CapitalCost / (1 + fees);

                        var sellOrder = client.PlaceOrder(trade.CoinPair.ToString(), OrderSide.Sell, OrderType.Limit, quantity: trade.CoinQuantity, price: trade.MarketPrice, timeInForce: TimeInForce.GoodTillCancel);
                        if (sellOrder.Success)
                        {
                            trade.SellPriceAfterFees = Math.Round(myInvestment / trade.CoinQuantity, 2);
                            trade.SellOrderID = sellOrder.Data.OrderId;
                            Bot.WriteLog(trade.ToStringSold());
                            Bot.Settings.TotalProfit += trade.Profit;
                            OnOrderSucceeded(trade);
                        }
                    }
                }
            }
        }

        #region Event Handlers

        private static void OnStarted()
        {
            if (Started != null)
            {
                ThreadWorker.InvokeAsync(() => Started());
            }
        }

        private static void OnPriceChecked(TradingData data)
        {
            if (PriceChecked != null)
            {
                ThreadWorker.InvokeAsync(() => PriceChecked(data));
            }
        }

        private static void OnOrderSucceeded(TradingData data)
        {
            if (OrderSucceeded != null)
            {
                ThreadWorker.InvokeAsync(() => OrderSucceeded(data));
            }
        }

        #endregion Event Handlers
    }
}