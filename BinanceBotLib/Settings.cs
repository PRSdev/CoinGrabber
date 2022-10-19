﻿using ExchangeClientLib;
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

        [Category("3 Fixed Price Change"), Description("Interval in seconds.")]
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

        [Category("5 Futures"), Description("If FuturesSafetyFactor is 15 then balance divided by 11 will be used for investment")]
        public virtual double FuturesSafetyFactor { get; set; } = 15;

        [Category("5 Futures"), Description("Take profit by a fixed profit target or by short/long price range")]
        public virtual FuturesTakeProfitMode TakeProfitMode { get; set; } = FuturesTakeProfitMode.ProfitByTarget;

        [Category("5 Futures"), Description("Automatically adjust Long Below and Short Above prices")]
        public virtual bool IsAutoAdjustShortAboveAndLongBelow { get; set; } = false;

        [Category("5 Futures"), Description("Short/Sell above this price")]
        public virtual double ShortAbove { get; set; } = 10000.0;

        [Category("5 Futures"), Description("Long/Buy below this price")]
        public virtual double LongBelow { get; set; } = 9500.0;

        [Category("5 Futures"), Description("Automatically determine target profit (Size / Levarage * Mark Price * 0.618)")]
        public virtual bool IsAutoAdjustTargetProfit { get; set; } = true;

        [Category("5 Futures"), Description("Target profit to close position. When above 0, closing position based on target profit has precedence over closing position based on price ranges.")]
        public virtual double FuturesProfitTarget { get; set; }
    }
}