using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public class MockupExchangeClient : ExchangeClient
    {
        public override decimal GetBalance(string coinPair)
        {
            return 0m;
        }
    }
}