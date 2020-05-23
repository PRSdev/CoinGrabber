using ExchangeClientLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BinanceBotLib
{
    public class Settings : SettingsBase<Settings>

    {
        [Category("1 General"), Browsable(false)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Category("1 General"), Description("Binance API Key.")]
        public string APIKey { get; set; }

        [Category("1 General"), Description("Binance Secret Key.")]
        public string SecretKey { get; set; }

        [Category("1 General"), Description("Orders will be actually placed during the production mode.")]
        public bool ProductionMode { get; set; } = false;

        [Category("1 General"), Browsable(false), Description("Total Profit.")]
        public decimal TotalProfit { get; set; }
        [Category("1 General"), Browsable(false), Description("Total Profit for Simulation.")]
        public decimal TotalProfitSimulation { get; set; }

        [Category("1 General"), Browsable(false)]
        public BotMode BotMode { get; set; } = BotMode.FixedPriceChange;

        [Category("2 Fixed Profit"), Description("Maximum investment.")]
        public decimal InvestmentMax { get; set; } = 500m;

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

        [Category("3 Fixed Price Change"), Description("Let the bot decide which coin to buy (BNB, BTC or BCH).")]
        public bool RandomNewCoinPair { get; set; } = false;

        [Category("3 Fixed Price Change"), DefaultValue(100), Description("Minimum capital investment in USDT")]
        public decimal InvestmentMin { get; set; } = 100m;

        [Category("3 Fixed Price Change"), Description("The subdivision number for your coin balance e.g. if you have 600 USDT as balance, HydraFactor of 3 will make Bot only invest 200 USDT.")]
        public int HydraFactor { get; set; } = 10;

        [Category("3 Fixed Price Change"), Description("When current price is below this percentage, new buy orders trigger.")]
        public decimal PriceChangePercentageDown { get; set; } = 4m;

        [Category("3 Fixed Price Change"), Description("When current price is above this percentage, new sell orders trigger.")]
        public decimal PriceChangePercentageUp { get; set; } = 2m;

        [Category("3 Fixed Price Change"), Description("Interval in minutes.")]
        public int TimerInterval { get; set; } = 180;

        [Category("3 Fixed Price Change"), Browsable(false)]
        public List<TradingData> TradingDataList { get; set; } = new List<TradingData>();

        [Category("4 Trading View")]
        public string GmailAddress { get; set; } = "user@gmail.com";

        [Category("4 Trading View"), PasswordPropertyTextAttribute(true)]
        public string GmailPassword { get; set; } = "MyPassword";

        [Category("4 Trading View"), Description("Secret word of your choice for anti-phishing purposes. Input the same secrete word in your Trading View account alert message.")]
        public string SecretWord { get; set; } = "MySecretWord";

        [Category("4 Trading View"), Browsable(false)]
        public DateTime LastEmailDateTime { get; set; } = DateTime.Now;

        [Category("4 Trading View"), Description("If set to 10, then the bot will sell if the market price goes 10% below buy price.")]
        public decimal StopLossPerc { get; set; } = 2m;

        [Category("4 Trading View"), Description("Close trade fully on sell signal (without taking partial profits or losses).")]
        public bool SellAllOnSellSignal { get; set; } = false;

        [Category("4 Trading View"), Description("If set to 25, then 25% of the quantity will be sold on the sell signal.")]
        public decimal SellQuantityPerc { get; set; } = 25m;

        [Category("4 Trading View"), Description("If set to 50, then max 50% of the quantity will be sold on the sell signal.")]
        public decimal SellMaxQuantityPerc { get; set; } = 50m;

        [Category("4 Trading View"), Browsable(false)]
        public List<TradingData> TradingViewTradesList { get; set; } = new List<TradingData>();
    }
}