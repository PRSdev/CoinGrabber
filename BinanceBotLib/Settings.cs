using Binance.Net.Objects;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;

namespace BinanceBotLib
{
    public class Settings : SettingsBase<Settings>
    {
        public string APIKey { get; set; }
        public string SecretKey { get; set; }

        // Day Trade Settings
        public decimal InvestmentMax { get; set; } = 500;
        public decimal DailyProfitTarget { get; set; } = 10;

        public long LastBuyOrderID { get; set; }
        public long LastSellOrderID { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal CoinQuantity { get; set; }

        // Swing Trade Settings
        public List<BinanceOrder> OrdersList { get; set; } = new List<BinanceOrder>();
        public decimal PriceChange { get; set; } = 4.0m;
        public decimal BuyBelow { get; set; } = 6500m;
        public decimal SellAbove { get; set; } = 7000m;
    }
}