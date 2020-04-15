using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BinanceBotLib
{
    public class FixedPriceChangeStrategy : Strategy
    {
        public FixedPriceChangeStrategy(ExchangeType exchangeType) : base(exchangeType)
        {
        }

        public override void Trade()
        {
            // Check USDT and BTC balances
            decimal coins = _client.GetBalance(Bot.Settings.CoinPair.Pair1);
            decimal fiatValue = _client.GetBalance(Bot.Settings.CoinPair.Pair2);

            // Check if user has more USDT or more BTC
            decimal coinsValue = coins * _client.GetPrice(Bot.Settings.CoinPair);

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
                    PlaceBuyOrder(GetNewTradingData());
                }
                else
                {
                    // sell
                    TradingData trade0 = GetNewTradingData();
                    trade0.CoinQuantity = Math.Round(coins / Bot.Settings.HydraFactor, 5);
                    Bot.Settings.TradingDataList.Add(trade0); // Add because this is the seed
                    PlaceSellOrder(trade0);
                }
            }
            else
            {
                // monitor market price for price changes
                Console.WriteLine();
                OnStarted();
                foreach (TradingData trade in Bot.Settings.TradingDataList)
                {
                    trade.MarketPrice = _client.GetPrice(trade.CoinPair);
                    trade.PriceChangePercentage = Math.Round((trade.MarketPrice - trade.BuyPriceAfterFees) / trade.BuyPriceAfterFees * 100, 2);
                    Console.WriteLine(trade.ToStringPriceCheck());
                    OnPriceChecked(trade);
                    // sell if positive price change
                    if (trade.PriceChangePercentage > Bot.Settings.PriceChangePercentage)
                    {
                        PlaceSellOrder(trade);
                    }
                    Thread.Sleep(200);
                }

                if (Bot.Settings.TradingDataList.Last<TradingData>().PriceChangePercentage < Bot.Settings.PriceChangePercentage * -1)
                {
                    // buy more if negative price change
                    PlaceBuyOrder(GetNewTradingData());
                }
            }

            OnCompleted();
        }

        public void PlaceBuyOrder(TradingData trade)
        {
            decimal fiatValue = _client.GetBalance(trade.CoinPair.Pair2);

            trade.CapitalCost = Math.Round(fiatValue / Bot.Settings.HydraFactor, 2);
            if (trade.CapitalCost > Bot.Settings.InvestmentMin)
            {
                trade.MarketPrice = Math.Round(_client.GetPrice(trade.CoinPair) * (1 - Math.Abs(Bot.Settings.BuyBelowPerc) / 100), 2);

                Console.WriteLine();

                decimal fees = _client.GetTradeFee(trade.CoinPair);
                decimal myInvestment = trade.CapitalCost / (1 + fees);
                trade.CoinQuantity = Math.Round(myInvestment / trade.MarketPrice, 5);

                var buyOrder = _client.PlaceBuyOrder(trade);
                if (buyOrder.Success)
                {
                    trade.BuyPriceAfterFees = Math.Round(trade.CapitalCost / trade.CoinQuantity, 2);
                    trade.BuyOrderID = buyOrder.Data.OrderId;
                    trade.ID = Bot.Settings.TradingDataList.Count;
                    Bot.Settings.TradingDataList.Add(trade);
                    Bot.WriteLog(trade.ToStringBought());
                    OnOrderSucceeded(trade);
                }
                else
                {
                    Bot.WriteLog(buyOrder.Error.Message.ToString());
                }
            }
            else
            {
                Console.WriteLine($"Capital cost is too low to buy more.");
            }
        }

        private void PlaceSellOrder(TradingData trade)
        {
            trade.MarketPrice = Math.Round(_client.GetPrice(trade.CoinPair) * (1 + Math.Abs(Bot.Settings.SellAbovePerc) / 100), 2);

            if (trade.MarketPrice > trade.BuyPriceAfterFees)
            {
                trade.CapitalCost = trade.CoinQuantity * trade.MarketPrice;

                if (trade.CapitalCost > Bot.Settings.InvestmentMin)
                {
                    decimal fees = _client.GetTradeFee(trade.CoinPair);
                    decimal myInvestment = trade.CapitalCost / (1 + fees);

                    var sellOrder = _client.PlaceSellOrder(trade);
                    if (sellOrder.Success)
                    {
                        trade.SellPriceAfterFees = Math.Round(myInvestment / trade.CoinQuantity, 2);
                        trade.SellOrderID = sellOrder.Data.OrderId;
                        Bot.WriteLog(trade.ToStringSold());
                        if (trade.BuyPriceAfterFees > 0) Bot.Settings.TotalProfit += trade.Profit;
                        OnOrderSucceeded(trade);
                    }
                }
            }
        }

        public TradingData GetNewTradingData()
        {
            return new TradingData() { CoinPair = CoinPairs.GetCoinPair() };
        }
    }
}