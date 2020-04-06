using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinanceBotLib
{
    public static class TradingHelper
    {
        public static void SwingTrade()
        {
            // Check USDT and BTC balances
            using (var client = new BinanceClient())
            {
                var accountInfo = client.GetAccountInfo();
                decimal coinsUSDT = accountInfo.Data.Balances.Single(s => s.Asset == Coins.USDT).Free;
                decimal coinsBTC = accountInfo.Data.Balances.Single(s => s.Asset == Coins.BTC).Free;

                // Check if user has more USDT or more BTC
                decimal marketBuyPrice = client.GetPrice(CoinPairs.BTCUSDT).Data.Price;
                decimal btcValue = marketBuyPrice * coinsBTC;

                // If no buy or sell orders, then place an order
                if (Bot.Settings.OrdersList.Count == 0)
                {
                    // buy or sell?
                    if (coinsUSDT > btcValue)
                    {
                        // buy
                        if (marketBuyPrice < Bot.Settings.BuyBelow)
                        {
                        }
                        Console.WriteLine($"USDT value: {coinsUSDT}");
                    }
                    else
                    {
                        // sell
                        if (marketBuyPrice > Bot.Settings.SellAbove)
                        {
                        }
                        Console.WriteLine($"BTC value: {btcValue}");
                    }
                }
            }
        }
    }
}