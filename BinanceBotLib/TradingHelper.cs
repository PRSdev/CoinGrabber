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
    }

    public static class TradingHelper
    {
        public static void BuyOrderDayTrade()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                decimal coinsUSDT = accountInfo.Data.Balances.Single(s => s.Asset == Coins.USDT).Free;

                decimal myCapitalCost = Bot.Settings.InvestmentMax == 0 ? coinsUSDT : Math.Min(Bot.Settings.InvestmentMax, coinsUSDT);
                Bot.WriteLog("USDT balance to trade = " + myCapitalCost.ToString());

                decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == CoinPairs.BTCUSDT).MakerFee;
                decimal myInvestment = myCapitalCost / (1 + fees);

                decimal myRevenue = myCapitalCost + Bot.Settings.DailyProfitTarget;
                Bot.WriteLog($"Receive target = {myRevenue}");

                // New method from: https://docs.google.com/spreadsheets/d/1be6zYuzKyJMZ4Yn_pUmIt-YRTON3YJKbq_lenL2Kldc/edit?usp=sharing
                decimal marketBuyPrice = client.GetPrice(CoinPairs.BTCUSDT).Data.Price;
                Bot.WriteLog($"Market price = {marketBuyPrice}");

                decimal marketSellPrice = myRevenue * (1 + fees) / (myInvestment / marketBuyPrice);
                decimal priceDiff = marketSellPrice - marketBuyPrice;

                Bot.Settings.BuyPrice = Math.Round(marketBuyPrice - priceDiff / 2, 2);
                Bot.Settings.CoinQuantity = Math.Round(myInvestment / Bot.Settings.BuyPrice, 6);

                Bot.WriteLog($"Buying {Bot.Settings.CoinQuantity} BTC for {Bot.Settings.BuyPrice}");
                var buyOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Buy, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: Bot.Settings.BuyPrice, timeInForce: TimeInForce.GoodTillCancel);

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
                decimal coinsBTC = accountInfo.Data.Balances.Single(s => s.Asset == Coins.BTC).Free;

                // if user has BTC rather than USDT for capital, then calculate SellPrice and CoinQuanitity
                if (Bot.Settings.SellPrice == 0 || Bot.Settings.CoinQuantity == 0)
                {
                    decimal marketBuyPrice = client.GetPrice(CoinPairs.BTCUSDT).Data.Price;
                    decimal myCapitalCost = Bot.Settings.InvestmentMax == 0 ? marketBuyPrice * coinsBTC : Math.Min(Bot.Settings.InvestmentMax, marketBuyPrice * coinsBTC);

                    decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == CoinPairs.BTCUSDT).MakerFee;
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
                    var sellOrder = client.PlaceOrder(CoinPairs.BTCUSDT, OrderSide.Sell, OrderType.Limit, quantity: Bot.Settings.CoinQuantity, price: Bot.Settings.SellPrice, timeInForce: TimeInForce.GoodTillCancel);

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
                decimal coinsUSDT = accountInfo.Data.Balances.Single(s => s.Asset == Coins.USDT).Free;
                decimal coinsBTC = accountInfo.Data.Balances.Single(s => s.Asset == Coins.BTC).Free;

                // Check if user has more USDT or more BTC
                decimal marketPrice = client.GetPrice(CoinPairs.BTCUSDT).Data.Price;
                decimal btcValue = marketPrice * coinsBTC;

                // cleanup
                Bot.Settings.TradingDataList.RemoveAll(trade => trade.BuyOrderID > -1 && trade.SellOrderID > -1);

                // If no buy or sell orders, then place an order
                if (Bot.Settings.TradingDataList.Count == 0)
                {
                    // buy or sell?
                    if (coinsUSDT > btcValue)
                    {
                        // buy
                        if (marketPrice < Bot.Settings.BuyBelow)
                        {
                            BuyOrderSwingTrade(marketPrice);
                        }
                    }
                    else
                    {
                        // sell
                        if (marketPrice > Bot.Settings.SellAbove)
                        {
                            TradingData trade0 = new TradingData();
                            trade0.CoinQuantity = Math.Round(coinsBTC / Bot.Settings.HydraFactor, 6);
                            Bot.Settings.TradingDataList.Add(trade0);
                            SellOrderSwingTrade(trade0, marketPrice);
                        }
                        Console.WriteLine($"BTC value: {btcValue}");
                    }
                }
                else
                {
                    // monitor market price for price changes
                    Console.WriteLine();
                    Console.WriteLine(marketPrice);
                    foreach (TradingData trade in Bot.Settings.TradingDataList)
                    {
                        trade.PriceChangePercentage = Math.Round((marketPrice - trade.BuyPriceAfterFees) / trade.BuyPriceAfterFees * 100, 2);
                        Console.WriteLine($"ID={trade.ID} Price={trade.BuyPriceAfterFees} Change={trade.PriceChangePercentage}%");

                        // sell if positive price change
                        if (trade.PriceChangePercentage > Bot.Settings.PriceChangePercentage)
                        {
                            SellOrderSwingTrade(trade, marketPrice);
                        }
                        Thread.Sleep(200);
                    }

                    if (Bot.Settings.TradingDataList.Last<TradingData>().PriceChangePercentage < Bot.Settings.PriceChangePercentage * -1)
                    {
                        // buy more if negative price change
                        BuyOrderSwingTrade(marketPrice);
                    }
                }
            }
        }

        public static void BuyOrderSwingTrade(decimal marketPrice)
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                decimal coinsUSDT = accountInfo.Data.Balances.Single(s => s.Asset == Coins.USDT).Free;

                TradingData trade = new TradingData();
                trade.CapitalCost = coinsUSDT / Bot.Settings.HydraFactor;
                trade.ID = Bot.Settings.TradingDataList.Count;
                Bot.Settings.TradingDataList.Add(trade);

                BuyOrderSwingTrade(trade, marketPrice);
            }
        }

        public static void BuyOrderSwingTrade(TradingData trade, decimal marketPrice)
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                Console.WriteLine();
                Bot.WriteLog("USDT balance to trade = " + Math.Round(trade.CapitalCost, 2));

                decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == CoinPairs.BTCUSDT).MakerFee;
                decimal myInvestment = trade.CapitalCost / (1 + fees);
                trade.CoinQuantity = Math.Round(myInvestment / marketPrice, 6);

                var buyOrder = client.PlaceTestOrder(CoinPairs.BTCUSDT, OrderSide.Buy, OrderType.Limit, quantity: trade.CoinQuantity, price: marketPrice, timeInForce: TimeInForce.GoodTillCancel);
                if (buyOrder.Success)
                {
                    trade.BuyPriceAfterFees = Math.Round(trade.CapitalCost / trade.CoinQuantity, 2);
                    trade.BuyOrderID = buyOrder.Data.OrderId;
                    Bot.WriteLog($"ID={trade.ID} Bought {trade.CoinQuantity} BTC using {trade.CapitalCost} for {marketPrice}");
                }
            }
        }

        public static void SellOrderSwingTrade(TradingData trade, decimal marketPrice)
        {
            using (var client = new BinanceClient())
            {
                trade.CapitalCost = trade.CoinQuantity * marketPrice;

                if (trade.CapitalCost > Bot.Settings.InvestmentMin)
                {
                    decimal fees = client.GetTradeFee().Data.Single(s => s.Symbol == CoinPairs.BTCUSDT).MakerFee;
                    decimal myInvestment = trade.CapitalCost / (1 + fees);

                    Bot.WriteLog("BTC to trade = " + trade.CoinQuantity.ToString());

                    var sellOrder = client.PlaceTestOrder(CoinPairs.BTCUSDT, OrderSide.Sell, OrderType.Limit, quantity: trade.CoinQuantity, price: marketPrice, timeInForce: TimeInForce.GoodTillCancel);
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