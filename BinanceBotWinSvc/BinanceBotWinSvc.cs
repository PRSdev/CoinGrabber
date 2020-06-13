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
        private Settings _settings;
        private Bot _bot;

        public BinanceBotWinSvc()
        {
            InitializeComponent();
        }

        // sc create BinanceBotWinSvc binPath="C:\Program Files\BinanceBotWinSvc.exe" obj=".\mike" password="password" start=auto

        protected override void OnStart(string[] args)
        {
            _settings = Bot.LoadSettings();
            _bot = new Bot(_settings);
            _bot.Start(_settings);

            _bot.Strategy.TradeListItemHandled += Strategy_TradeListItemHandled;
        }

        private void Strategy_TradeListItemHandled(TradingData data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Entry Price: {data.BuyPriceAfterFees}");
            if (data.PriceLongBelow > 0)
            {
                sb.AppendLine($"Long Below: {data.PriceLongBelow}");
                sb.AppendLine($"Short Above: {data.PriceShortAbove}");
                sb.AppendLine($"Target Profit: {data.ProfitTarget}");
            }
            EventLog.WriteEntry("BinanceBotWinSvc", sb.ToString());
        }

        protected override void OnContinue()
        {
        }

        protected override void OnStop()
        {
        }
    }
}