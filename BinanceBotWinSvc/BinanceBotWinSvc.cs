using BinanceBotLib;
using ExchangeClientLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BinanceBotWinSvc
{
    public partial class BinanceBotWinSvc : ServiceBase
    {
        private Bot _bot;

        public BinanceBotWinSvc()
        {
            InitializeComponent();
        }

        // sc create BinanceBotWinSvc binPath="C:\Program Files\BinanceBotWinSvc.exe" obj=".\mike" password="password" start=auto

        protected override void OnStart(string[] args)
        {
            _bot = new Bot();
            _bot.Start();

            _bot.Strategy.TradeListItemHandled += Strategy_TradeListItemHandled;
        }

        private void Strategy_TradeListItemHandled(TradingData data)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Market Price: {data.Price}");
            sb.AppendLine($"Entry Price: {data.BuyPriceAfterFees}");
            if (data.PriceLongBelow > 0)
            {
                sb.AppendLine($"Long Below: {data.PriceLongBelow}");
                sb.AppendLine($"Short Above: {data.PriceShortAbove}");
                sb.AppendLine($"Target Profit: {data.ProfitTarget}");
            }
            EventLog.WriteEntry("BinanceBotWinSvc", _bot.ToStatusString(data));
        }

        protected override void OnContinue()
        {
        }

        protected override void OnStop()
        {
        }
    }
}