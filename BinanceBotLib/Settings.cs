using ShareX.HelpersLib;
using System;

namespace BinanceBotLib
{
    public class Settings : SettingsBase<Settings>
    {
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public decimal InvestmentMax { get; set; } = 500;
        public decimal DailyProfitTarget { get; set; } = 10;

        public long LastOrderID { get; set; }
        public decimal SellPrice { get; set; }
        public decimal CoinQuantity { get; set; }
    }
}