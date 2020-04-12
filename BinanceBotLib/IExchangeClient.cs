using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBotLib
{
    public interface IExchangeClient
    {
        void Init();

        void Start();

        void SwingTrade();
    }
}