using Binance.Net.Clients;
using Binance.Net.Enums;
using System;
using System.Linq;

namespace BinanceBotLib
{
    public class FixedProfitStrategy : Strategy
    {
        public FixedProfitStrategy(ExchangeType exchange, Settings settings) : base(exchange, settings)
        {
        }

        public override void Trade()
        {
            using (var client = new BinanceClient())
            {
                var queryBuyOrder = client.SpotApi.Trading.GetOrderAsync(_settings.CoinPair.ToString(), orderId: _settings.LastBuyOrderID); ;
                var querySellOrder = client.SpotApi.Trading.GetOrderAsync(_settings.CoinPair.ToString(), orderId: _settings.LastSellOrderID);

                if (queryBuyOrder.Result.Data != null)
                {
                    switch (queryBuyOrder.Result.Data.Status)
                    {
                        case OrderStatus.Filled:
                            PlaceSellOrder();
                            break;
                        case OrderStatus.Canceled:
                            Bot.WriteLog($"Buy order {_settings.LastBuyOrderID} has been cancelled by the user.");
                            PlaceBuyOrder();
                            break;
                        case OrderStatus.New:
                            Bot.WriteConsole($"Waiting {DateTime.UtcNow - queryBuyOrder.Result.Data.CreateTime} for the {_settings.BuyPrice} buy order to fill...");
                            break;
                        default:
                            Bot.WriteConsole("Unhandled buy order outcome. Reload application...");
                            break;
                    }
                }
                else if (querySellOrder.Result.Data != null)
                {
                    switch (querySellOrder.Result.Data.Status)
                    {
                        case OrderStatus.Filled:
                            PlaceBuyOrder();
                            break;
                        case OrderStatus.Canceled:
                            Bot.WriteLog($"Sell order {_settings.LastSellOrderID} has been cancelled by the user.");
                            PlaceSellOrder();
                            break;
                        case OrderStatus.New:
                            Bot.WriteConsole($"Waiting {DateTime.UtcNow - querySellOrder.Result.Data.CreateTime} for the {_settings.SellPrice} sell order to fill...");
                            break;
                        default:
                            Bot.WriteConsole("Unhandled sell order outcome. Reload application...");
                            break;
                    }
                }
                else if (queryBuyOrder.Result.Data == null)
                {
                    Bot.WriteConsole("Could not find any previous buy orders.");
                    PlaceBuyOrder();
                }
                else if (querySellOrder.Result.Data == null)
                {
                    Bot.WriteConsole("Could not find any previous sell orders.");
                    PlaceSellOrder();
                }
            }
        }

        public override void PlaceBuyOrder()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.SpotApi.Account.GetAccountInfoAsync();
                decimal coinsUSDT = accountInfo.Result.Data.Balances.Single(s => s.Asset == _settings.CoinPair.Pair2).Available;

                decimal myCapitalCost = _settings.InvestmentMax == 0 ? coinsUSDT : Math.Min(_settings.InvestmentMax, coinsUSDT);
                Bot.WriteLog("USDT balance to trade = " + myCapitalCost.ToString());

                decimal fees = client.SpotApi.ExchangeData.GetTradeFeeAsync().Result.Data.Single(s => s.Symbol == _settings.CoinPair.ToString()).MakerFee;
                decimal myInvestment = myCapitalCost / (1 + fees);

                decimal myRevenue = myCapitalCost + _settings.DailyProfitTarget;
                Bot.WriteLog($"Receive target = {myRevenue}");

                // New method from: https://docs.google.com/spreadsheets/d/1be6zYuzKyJMZ4Yn_pUmIt-YRTON3YJKbq_lenL2Kldc/edit?usp=sharing
                decimal marketBuyPrice = client.SpotApi.ExchangeData.GetPriceAsync(_settings.CoinPair.ToString()).Result.Data.Price;
                Bot.WriteLog($"Market price = {marketBuyPrice}");

                decimal marketSellPrice = myRevenue * (1 + fees) / (myInvestment / marketBuyPrice);
                decimal priceDiff = marketSellPrice - marketBuyPrice;

                _settings.BuyPrice = Math.Round(marketBuyPrice - priceDiff / 2, 2);
                _settings.CoinQuantity = Math.Round(myInvestment / _settings.BuyPrice, 6);

                Bot.WriteLog($"Buying {_settings.CoinQuantity} {_settings.CoinPair.Pair1} for {_settings.BuyPrice}");
                var buyOrder = client.SpotApi.Trading.PlaceTestOrderAsync(_settings.CoinPair.ToString(), OrderSide.Buy, SpotOrderType.Limit, quantity: _settings.CoinQuantity, price: _settings.BuyPrice, timeInForce: TimeInForce.GoodTillCanceled);

                if (buyOrder.Result.Success)
                {
                    // Save Sell Price
                    _settings.SellPrice = Math.Round(myRevenue * (1 + fees) / _settings.CoinQuantity, 2);
                    Bot.WriteLog("Target sell price = " + _settings.SellPrice);

                    decimal priceChange = Math.Round(priceDiff / _settings.BuyPrice * 100, 2);
                    Bot.WriteConsole($"Price change = {priceChange}%");
                    _settings.LastBuyOrderID = buyOrder.Result.Data.Id;
                    Bot.WriteLog("Order ID: " + buyOrder.Result.Data.Id);
                    Bot.WriteConsole();
                }
            }
        }

        public override void PlaceSellOrder()
        {
            using (var client = new BinanceClient())
            {
                var accountInfo = client.SpotApi.Account.GetAccountInfoAsync();
                decimal coinsQuantity = accountInfo.Result.Data.Balances.Single(s => s.Asset == _settings.CoinPair.Pair1).Available;

                // if user has crypto rather than USDT for capital, then calculate SellPrice and CoinQuanitity
                if (_settings.SellPrice == 0 || _settings.CoinQuantity == 0)
                {
                    decimal marketBuyPrice = client.SpotApi.ExchangeData.GetPriceAsync(_settings.CoinPair.ToString()).Result.Data.Price;
                    decimal myCapitalCost = _settings.InvestmentMax == 0 ? marketBuyPrice * coinsQuantity : Math.Min(_settings.InvestmentMax, marketBuyPrice * coinsQuantity);

                    decimal fees = client.SpotApi.ExchangeData.GetTradeFeeAsync().Result.Data.Single(s => s.Symbol == _settings.CoinPair.ToString()).MakerFee;
                    decimal myInvestment = myCapitalCost / (1 + fees);

                    decimal myRevenue = myCapitalCost + _settings.DailyProfitTarget;

                    decimal marketSellPrice = myRevenue * (1 + fees) / (myInvestment / marketBuyPrice);
                    decimal priceDiff = marketSellPrice - marketBuyPrice;

                    _settings.BuyPrice = Math.Round(marketBuyPrice - priceDiff / 2, 2);
                    _settings.CoinQuantity = Math.Round(myInvestment / _settings.BuyPrice, 6);
                    _settings.SellPrice = Math.Round(myRevenue * (1 + fees) / _settings.CoinQuantity, 2);
                }

                if (_settings.SellPrice > 0 && _settings.CoinQuantity > 0 && coinsQuantity > _settings.CoinQuantity)
                {
                    var sellOrder = client.SpotApi.Trading.PlaceTestOrderAsync(_settings.CoinPair.ToString(), OrderSide.Sell, SpotOrderType.Limit, quantity: _settings.CoinQuantity, price: _settings.SellPrice, timeInForce: TimeInForce.GoodTillCanceled);

                    if (sellOrder.Result.Success)
                    {
                        Bot.WriteLog($"Sold {_settings.CoinQuantity} {_settings.CoinPair.Pair1} for {_settings.SellPrice}");
                        _settings.LastSellOrderID = sellOrder.Result.Data.Id;
                        Bot.WriteLog("Order ID: " + sellOrder.Result.Data.Id);
                        _settings.TotalProfit += _settings.DailyProfitTarget;
                        Bot.WriteConsole();
                    }
                }
            }
        }
    }
}