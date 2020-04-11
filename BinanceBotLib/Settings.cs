using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BinanceBotLib
{
    public class Settings : SettingsBase<Settings>
    {
        [Category("1 General"), Description("Binance API Key.")]
        public string APIKey { get; set; }

        [Category("1 General"), Description("Binance Secret Key.")]
        public string SecretKey { get; set; }

        [Category("1 General"), Browsable(false), Description("Total Profit.")]
        public decimal TotalProfit { get; set; }

        [Category("1 General"), Browsable(false)]
        public BotMode BotMode { get; set; } = BotMode.SwingTrade;

        [Category("2 Day Trade"), Description("Maximum investment.")]
        public decimal InvestmentMax { get; set; } = 500;

        [Category("2 Day Trade"), Description("Daily profit target.")]
        public decimal DailyProfitTarget { get; set; } = 10;

        [Category("2 Day Trade / Last Trade"), Browsable(false)]
        public long LastBuyOrderID { get; set; }

        [Category("2 Day Trade / Last Trade"), Browsable(false)]
        public long LastSellOrderID { get; set; }

        [Category("2 Day Trade / Last Trade"), Browsable(false)]
        public decimal BuyPrice { get; set; }

        [Category("2 Day Trade / Last Trade"), Browsable(false)]
        public decimal SellPrice { get; set; }

        [Category("2 Day Trade / Last Trade"), Browsable(false)]
        public decimal CoinQuantity { get; set; }

        [Category("3 Swing Trade")]
        public CoinPair CoinPair { get; set; } = new CoinPair("BTC", "USDT");

        [Category("3 Swing Trade"), Description("Minimum capital investment in USDT")]
        public decimal InvestmentMin { get; set; } = 100;

        [Category("3 Swing Trade"), Description("The subdivision number for your coin balance e.g. if you have 600 USDT as balance, HydraFactor of 3 will make Bot only invest 200 USDT.")]
        public int HydraFactor { get; set; } = 3;

        [Category("3 Swing Trade"), Description("When current price is below or above this percentage, buy or sell order triggers.")]
        public decimal PriceChangePercentage { get; set; } = 1.67m;

        [Category("3 Swing Trade"), Description("Do not buy above this price")]
        public decimal BuyBelow { get; set; } = 9000m;

        [Category("3 Swing Trade"), Description("Do not sell below this price")]
        public decimal SellAbove { get; set; } = 10000m;

        [Category("3 Swing Trade"), Browsable(false)]
        public List<TradingData> TradingDataList { get; set; } = new List<TradingData>();
    }
}