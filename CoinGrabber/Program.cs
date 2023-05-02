using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;

Settings _settings;
PeriodicTimer _waitTimer = new PeriodicTimer(new TimeSpan(0, 0, 5));
BinanceClient _client;

CoinPair _coinPair;
decimal _bidPrice;

_settings = SettingsManager.LoadSettings();
Console.WriteLine($"Settings filepath: {_settings.FilePath}");

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
_coinPair = new CoinPair(Console.ReadLine().Trim(), "USDT", 1); // Some coins only support one decimal

Console.Write("Enter your maximum price (1.00): ");
decimal.TryParse(Console.ReadLine().Trim(), out _bidPrice);

Console.Write("Listing date and time in UTC (2022-12-25 01:00): ");
DateTime coinListingUtcTime;
string strTime = Console.ReadLine().Trim();
if (!string.IsNullOrEmpty(strTime))
{
    DateTime.TryParse(strTime, out coinListingUtcTime);
    _settings.CoinListingUtcTime = coinListingUtcTime;
    SettingsManager.SaveSettings(_settings);
}

_client = new BinanceClient(new BinanceClientOptions
{
    ApiCredentials = new BinanceApiCredentials(_settings.APIKey, _settings.SecretKey),
    SpotApiOptions = new BinanceApiClientOptions
    {
        BaseAddress = BinanceApiAddresses.Default.RestClientAddress,
        AutoTimestamp = false
    },
});


while (await _waitTimer.WaitForNextTickAsync())
{
    if (_settings.CoinListingUtcTime - DateTime.UtcNow < new TimeSpan(0, 2, 0))
    {
        try
        {
            PeriodicTimer marketTimer = new PeriodicTimer(new TimeSpan(0, 0, 1));
            while (await marketTimer.WaitForNextTickAsync())
            {
                try
                {
                    if (PlaceBuyOrder())
                    {
                        marketTimer.Dispose();
                        _waitTimer.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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

bool PlaceBuyOrder()
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
                    return true;
                }
            }
        }
    }
    return false;
}