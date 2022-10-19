using ExchangeClientLib;
using ShareX.HelpersLib;
using System;
using System.ComponentModel;

namespace BinanceBotLib
{
    public class Settings : SettingsBase<Settings>

    {
        [Category("1 General"), Browsable(false)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Category("1 General"), Description("Binance API Key.")]
        public virtual string APIKey { get; set; }

        [Category("1 General"), Description("Binance Secret Key.")]
        public virtual string SecretKey { get; set; }

        [Category("0 General"), Description("Orders will be actually placed during the production mode.")]
        public bool ProductionMode { get; set; } = false;

        [Category("1 General"), Browsable(false), Description("Total Profit.")]
        public decimal TotalProfit { get; set; }
        [Category("1 General"), Browsable(false), Description("Total Profit for Simulation.")]
        public decimal TotalProfitSimulation { get; set; }

        [Category("1 General"), Browsable(false)]
        public BotMode BotMode { get; set; } = BotMode.FixedPrice;

        [Category("2 Fixed Price"), Description("Buy upto price.")]
        public decimal BuyUptoPrice { get; set; } = 1;

        [Category("2 Fixed Profit"), Description("Daily profit target.")]
        public decimal DailyProfitTarget { get; set; } = 10m;

        [Category("2 Fixed Profit / Last Trade"), Browsable(false)]
        public long LastBuyOrderID { get; set; }

        [Category("2 Fixed Profit / Last Trade"), Browsable(false)]
        public long LastSellOrderID { get; set; }

        [Category("2 Fixed Profit / Last Trade"), Browsable(false)]
        public decimal BuyPrice { get; set; }

        [Category("2 Fixed Profit / Last Trade"), Browsable(false)]
        public decimal SellPrice { get; set; }

        [Category("2 Fixed Profit / Last Trade"), Browsable(false)]
        public decimal CoinQuantity { get; set; }

        [Category("3 Fixed Price Change"), Browsable(false)]
        public CoinPair CoinPair { get; set; } = ExchangeClient.CoinPairsList[0];
    }
}