using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using BinanceBotLib;
using CryptoExchange.Net.Authentication;
using ExchangeClientLib;
using System;
using System.Linq;
using System.Timers;

namespace BinanceBotConsole
{
    internal class Program

    {
        private static Settings _settings { get; set; }
        private static Timer _marketTimer = new Timer();
        private static BinanceClient _client;

        private static CoinPair _coinPair;
        private static decimal _bidPrice;

        private static void Main(string[] args)
        {
            _settings = SettingsManager.LoadSettings();

            // Error handling
            if (string.IsNullOrEmpty(_settings.APIKey))
            {
                Console.Write("Enter Binance API Key: ");
                _settings.APIKey = Console.ReadLine().Trim();

                Console.Write("Enter Binance Secret Key: ");
                _settings.SecretKey = Console.ReadLine().Trim();

                SettingsManager.SaveSettings(_settings);
            }

            Console.Write("Enter coin to grab (SUI): ");
            _coinPair = new CoinPair(Console.ReadLine().Trim(), "BUSD", 1); // Some coins only support one decimal

            Console.Write("Enter your maximum price (1.00): ");
            decimal.TryParse(Console.ReadLine().Trim(), out _bidPrice);

            Console.Write("Listing date and time in UTC (2022-12-25 01:00): ");
            DateTime coinListingUtcTime;
            string strTime = Console.ReadLine().Trim();
            if (!string.IsNullOrEmpty(strTime))
            {
                DateTime.TryParse(strTime, out coinListingUtcTime);
                _settings.CoinListingUtcTime = coinListingUtcTime;
            }

            SettingsManager.SaveSettings(_settings);

            _client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(_settings.APIKey, _settings.SecretKey),
                SpotApiOptions = new BinanceApiClientOptions
                {
                    BaseAddress = BinanceApiAddresses.Default.RestClientAddress,
                    AutoTimestamp = false
                },
            });

            _marketTimer.Interval = 5000;
            _marketTimer.Elapsed += MarketTimer_Elapsed;
            _marketTimer.Start();

            Console.ReadLine();
        }

        private static void MarketTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_settings.CoinListingUtcTime - DateTime.UtcNow < new TimeSpan(0, 2, 0))
            {
                try
                {
                    PlaceBuyOrder();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Duration until listing: {_settings.CoinListingUtcTime - DateTime.UtcNow}");
            }
        }

        public static void PlaceBuyOrder()
        {
            decimal balance = _client.SpotApi.Account.GetAccountInfoAsync().Result.Data.Balances.Single(s => s.Asset == _coinPair.Pair2).Available;
            Console.WriteLine($"{_coinPair.Pair2} balance: {balance}");

            if (balance > 10)
            {
                decimal price = _client.SpotApi.ExchangeData.GetPriceAsync(_coinPair.ToString()).Result.Data.Price;
                Console.WriteLine($"{_coinPair} price: {price}");

                if (price > 0 && price <= _bidPrice)
                {
                    decimal quantity = Math.Floor(balance / price * 10) / 10; // Binance deducts the fees in coin quantity after buying
                    if (quantity > 0)
                    {
                        Console.WriteLine($"Max quantity to buy: {quantity.ToString()}");
                        var buyOrder = _client.SpotApi.Trading.PlaceOrderAsync(
                        _coinPair.ToString(),
                        OrderSide.Buy,
                        SpotOrderType.Market,
                        quantity: Math.Round(quantity, _coinPair.Precision));

                        if (buyOrder.Result.Success)
                        {
                            Console.WriteLine($"Success: {buyOrder.Result.Data.Id}");
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
    }
}