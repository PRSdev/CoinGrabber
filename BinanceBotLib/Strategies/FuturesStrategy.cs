using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceBotLib
{
    public class FuturesStrategy : Strategy
    {
        public FuturesStrategy(ExchangeType exchangeType, Settings settings) : base(exchangeType, settings)
        {
        }

        public override void Trade()
        {
            Console.WriteLine(_client.GetBalance("USDT"));
        }
    }
}