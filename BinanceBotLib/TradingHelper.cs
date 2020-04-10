using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BinanceBotLib
{
    public class TradingData
    {
        public int ID { get; set; }
        public CoinPair CoinPair { get; set; }
        public decimal CapitalCost { get; set; }
        public decimal CoinQuantity { get; set; }
        public decimal PriceChangePercentage { get; set; }
        public decimal BuyPriceAfterFees { get; set; }
        public decimal SellPriceAfterFees { get; set; }

        public long BuyOrderID { get; set; } = -1;
        public long SellOrderID { get; set; } = -1;

        public decimal Profit
        {
            get
            {
                return SellPriceAfterFees == 0 ? 0 : Math.Round((SellPriceAfterFees - BuyPriceAfterFees) * CoinQuantity, 2);
            }
        }

        public static TradingData GetNew()
        {
            return new TradingData() { CoinPair = Bot.Settings.CoinPair };
        }
    }

    public static class TradingHelper
    {
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

        public static void BuyOrderDayTrade()
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

                Bot.WriteLog($"Buying {Bot.Settings.CoinQuantity} BTC for {Bot.Settings.BuyPrice}");
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

        public static void SellOrderDayTrade()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                decimal coinsBTC = accountInfo.Data.Balances.Single(s => s.Asset == Bot.Settings.CoinPair.Pair1).Free;

                // if user has BTC rather than USDT for capital, then calculate SellPrice and CoinQuanitity
                if (Bot.Settings.SellPrice == 0 || Bot.Settings.CoinQuantity == 0)
                {
                    decimal marketBuyPrice = client.GetPrice(Bot.Settings.CoinPair.ToString()).Data.Price;
                    decimal myCapitalCost = Bot.Settings.InvestmentMax == 0 ? marketBuyPrice * coinsBTC : Math.Min(Bot.Settings.InvestmentMax, marketBuyPrice * coinsBTC);

                    decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == Bot.Settings.CoinPair.ToString()).MakerFee;
                    decimal myInvestment = myCapitalCost / (1 + fees);

                    decimal myRevenue = myCapitalCost + Bot.Settings.DailyProfitTarget;

                    decimal marketSellPrice = myRevenue * (1 + fees) / (myInvestment / marketBuyPrice);
                    decimal priceDiff = marketSellPrice - marketBuyPrice;

                    Bot.Settings.BuyPrice = Math.Round(marketBuyPrice - priceDiff / 2, 2);
                    Bot.Settings.CoinQuantity = Math.Round(myInvestment / Bot.Settings.BuyPrice, 6);
                    Bot.Settings.SellPrice = Math.Round(myRevenue * (1 + fees) / Bot.Settings.CoinQuantity, 2);
                }

                if (Bot.Settings.SellPrice > 0 && Bot.Settings.CoinQuantity > 0 && coinsBTC > Bot.Settings.CoinQuantity)
                {
                    var sellOrder = client.PlaceOrder(Bot.Settings.CoinPair.ToString(), OrderSide.Sell, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: Bot.Settings.SellPrice, timeInForce: TimeInForce.GoodTillCancel);

                    if (sellOrder.Success)
                    {
                        Bot.WriteLog($"Sold {Bot.Settings.CoinQuantity} BTC for {Bot.Settings.SellPrice}");
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
                    foreach (TradingData trade in Bot.Settings.TradingDataList)
                    {
                        decimal marketPrice = client.GetPrice(trade.CoinPair.ToString()).Data.Price;
                        Console.WriteLine($"{trade.CoinPair.ToString()} {marketPrice}");
                        trade.PriceChangePercentage = Math.Round((marketPrice - trade.BuyPriceAfterFees) / trade.BuyPriceAfterFees * 100, 2);
                        Console.WriteLine($"ID={trade.ID} Price={trade.BuyPriceAfterFees} Change={trade.PriceChangePercentage}%");
                        Console.WriteLine();

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
                decimal marketPrice = client.GetPrice(trade.CoinPair.ToString()).Data.Price;
                if (marketPrice < Bot.Settings.BuyBelow)
                {
                    Console.WriteLine();

                    decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == trade.CoinPair.ToString()).MakerFee;
                    decimal myInvestment = trade.CapitalCost / (1 + fees);
                    trade.CoinQuantity = Math.Round(myInvestment / marketPrice, 5);

                    var buyOrder = client.PlaceOrder(trade.CoinPair.ToString(), OrderSide.Buy, OrderType.Limit, quantity: trade.CoinQuantity, price: marketPrice, timeInForce: TimeInForce.GoodTillCancel);
                    if (buyOrder.Success)
                    {
                        trade.BuyPriceAfterFees = Math.Round(trade.CapitalCost / trade.CoinQuantity, 2);
                        trade.BuyOrderID = buyOrder.Data.OrderId;
                        trade.ID = Bot.Settings.TradingDataList.Count;
                        Bot.Settings.TradingDataList.Add(trade);
                        Bot.WriteLog($"ID={trade.ID} Bought {trade.CoinQuantity} BTC using {trade.CapitalCost} for {marketPrice}");
                    }
                }
            }
        }

        private static void SellOrderSwingTrade(TradingData trade)
        {
            using (var client = new BinanceClient())
            {
                decimal marketPrice = client.GetPrice(trade.CoinPair.ToString()).Data.Price;

                if (marketPrice > Bot.Settings.SellAbove)
                {
                    trade.CapitalCost = trade.CoinQuantity * marketPrice;

                    if (trade.CapitalCost > Bot.Settings.InvestmentMin)
                    {
                        decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == trade.CoinPair.ToString()).MakerFee;
                        decimal myInvestment = trade.CapitalCost / (1 + fees);

                        var sellOrder = client.PlaceOrder(trade.CoinPair.ToString(), OrderSide.Sell, OrderType.Limit, quantity: trade.CoinQuantity, price: marketPrice, timeInForce: TimeInForce.GoodTillCancel);
                        if (sellOrder.Success)
                        {
                            trade.SellPriceAfterFees = Math.Round(myInvestment / trade.CoinQuantity, 2);
                            trade.SellOrderID = sellOrder.Data.OrderId;
                            Bot.WriteLog($"ID={trade.ID} Sold {trade.CoinQuantity} BTC for {marketPrice} with profit {trade.Profit}");
                            Bot.Settings.TotalProfit += trade.Profit;
                        }
                    }
                }
            }
        }
    }
}