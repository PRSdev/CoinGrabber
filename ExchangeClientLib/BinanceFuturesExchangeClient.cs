using Binance.Net;
using Binance.Net.Objects.Futures;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace ExchangeClientLib
{
    public class BinanceFuturesExchangeClient : ExchangeClient
    {
        public BinanceFuturesExchangeClient(string apiKey, string secretKey) : base(apiKey, secretKey)
        {
            BinanceFuturesClient.SetDefaultOptions(new BinanceFuturesClientOptions()
            {
                ApiCredentials = new ApiCredentials(apiKey, secretKey),
                LogVerbosity = LogVerbosity.Error,
                LogWriters = new List<TextWriter> { Console.Out }
            });
        }

        public override decimal GetBalance(string coinName)
        {
            using (var client = new BinanceFuturesClient())
            {
                try
                {
                    decimal balance = client.GetAccountInfo().Data.TotalWalletBalance;
                    Portfolio.UpdateCoinBalance(coinName, balance);
                    return balance;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
    }
}