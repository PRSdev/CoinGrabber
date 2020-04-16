using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeClientLib
{
    public class MockupExchangeClient : ExchangeClient
    {
        public MockupExchangeClient(string apiKey, string secretKey) : base(apiKey, secretKey)
        {
        }

        public override decimal GetBalance(string coinPair)
        {
            return 0m;
        }
    }
}